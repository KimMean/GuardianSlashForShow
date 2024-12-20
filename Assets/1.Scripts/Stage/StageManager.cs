using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static Packet;

public class StageManager : MonoBehaviour
{
    private static StageManager instance;
    public static StageManager Instance
    {
        get { return instance; }
    }

    const int MAX_STAGE = 80;
    const int MAX_WAVE = 10;
    const int HALF_WAVE = 5;
    const int MAX_BLOCK_COUNT = 10;
    const int MAX_SHIELD_COUNT = 9;
    const int MAX_SHIELD_LEVEL = 10;
    const int BASE_PROBABILITY = 5;
    const int REWARD_MULTIPLIER = 3;
    const int BONUS_PROBABILITY = 5;    // ���ʽ� Ȯ��

    Camera mainCamera;

    [SerializeField] GameObject playerCharacterPrefab;
    GameObject playerCharacter;
    CharacterController characterController;
    [SerializeField] GameObject pausePanel;

    [SerializeField] GameObject blockParent;
    [SerializeField] GameObject blockPrefab;
    Sprite[] blockSprites = new Sprite[MAX_STAGE];
    Sprite[] shieldSprites = new Sprite[MAX_SHIELD_COUNT];
    GameObject[] blocks;
    BlockController[] blockController;
    [SerializeField] ParticleManager crashParticle;
    [SerializeField] ParticleManager absoluteDefenseParticle;
    [SerializeField] ParticleManager diamondParticle;

    public float cameraYOffset = 0f; // ī�޶� ���� ����
    bool isStart = false;

    int targetBlockIndex = 0;

    int characterLife;

    int currentStage = 0;
    int wave = 0;
    bool isClear;

    int blockDestroyCount = 0;
    int destroyCount = 0;

    int stageScore = 0;
    int stageCoin = 0;
    int stageDia = 0;
    int combo = 0;
    int maxCombo = 0;

    float extraCoin = 0;
    float deceleration = 0;
    float absoluteDefense = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        mainCamera = Camera.main;

        playerCharacter = Instantiate(playerCharacterPrefab, Vector3.zero, Quaternion.identity);
        characterController = playerCharacter.GetComponent<CharacterController>();

        currentStage = GameManager.Instance.GetCurrentStage();
        // ����� �ӵ��� �ʹ� ���� �ʹ� �����庮�� ����
        // ������������ ������ �������Բ� ����
        // �߷� ������ ��� (�⺻ �ӵ� + �������� �߰� �ӵ�)
        float gravityScale = 0.2f + (0.6f * (currentStage - 1) / (MAX_STAGE-1));
        //Debug.Log($"CurrentStage : {CurrentStage}, MAX STAGE : {MAX_STAGE}, GravityScale : {gravityScale}");
        blockParent.GetComponent<CustomPhysics>().SetGravityScale(gravityScale);


        for (int i = 1; i <= MAX_STAGE; i++)
        {
            blockSprites[i - 1] = Resources.Load<Sprite>("Blocks/Stage" + i.ToString("00"));
        }
        for (int i = 1; i <= MAX_SHIELD_COUNT; i++)
        {
            shieldSprites[i - 1] = Resources.Load<Sprite>("Shield/Shield" + i.ToString());
        }

        blocks = new GameObject[MAX_BLOCK_COUNT];
        blockController = new BlockController[MAX_BLOCK_COUNT];

        string ringCode = GameManager.Instance.GetEquipmentRing();
        if (ringCode != "R000")
            extraCoin = RingManager.Instance.GetRingData(ringCode).GetGold() / 100f;

        string necklaceCode = GameManager.Instance.GetEquipmentNecklace();
        if(necklaceCode != "N000")
        {
            absoluteDefense = NecklaceManager.Instance.GetNecklaceData(necklaceCode).GetTwilight();
        }
    }

    private void OnEnable()
    {
        UIManager.Instance.SetBlockHealthActive(false);

        isClear = false;
        wave = 0;
        characterLife = 3;
        stageScore = 0;
        blockDestroyCount = 0;
        destroyCount = 0;
        stageCoin = 0;
        stageDia = 0;
        combo = 0;
        maxCombo = 0;

        CreateBlocks();
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("StageStart");
        //StartCoroutine(StageStartDelay());
        StartCoroutine(GameStartDelay());
        SoundManager.Instance.ChangeBGM(SoundManager.BGM_Clip.Game);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(isStart)
        {
            // ĳ������ ���� ��ġ
            Vector3 characterPosition = playerCharacter.transform.position;
            // ī�޶��� ���� ��ġ
            Vector3 cameraPosition = mainCamera.transform.position;

            // ĳ���Ͱ� ���� ����(0)�� �Ѿ ��� ī�޶� ����
            if (characterPosition.y > cameraYOffset)
            {
                // ī�޶��� y���� ĳ������ y������ ������Ʈ
                cameraPosition.y = characterPosition.y;
            }
            else
            {
                // ī�޶� ���� ���� ���Ϸ� �������� ����
                cameraPosition.y = Mathf.Max(cameraYOffset, cameraPosition.y);
            }

            // ī�޶� ��ġ ������Ʈ
            mainCamera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                CharacterJump();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                CharacterDefend();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                CharacterAttack();
            }
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    OnBasicSkillKeyPressed?.Invoke();
            //}
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    OnSpacialSkillKeyPressed?.Invoke();
            //}
        }
    }

    public void CharacterJump()
    {
        characterController.Jump();
    }
    public void CharacterDefend()
    {
        characterController.Defend();
    }

    public void CharacterAttack()
    {
        characterController.Attack();
    }

    void OnDisable()
    {
    }

    
    /// <summary>
    /// ĳ���Ͱ� ���鿡 ������ �Ŀ� ���������� ���۵˴ϴ�.
    /// </summary>
    IEnumerator GameStartDelay()
    {
        CustomPhysics physics = playerCharacter.GetComponent<CustomPhysics>();

        while(!physics.GetIsGround())
        {
            yield return null;
        }

        // ĳ���Ͱ� ���鿡 �����ϸ� ����
        isStart = true;
        StartNextWave();
    }

    /// <summary>
    /// ������ �ش��ϴ� ��� �̹����� �����ɴϴ�.
    /// </summary>
    /// <param name="level">1~80</param>
    /// <returns>Block Sprite</returns>
    public Sprite GetBlockSprite(int level)
    {
        return blockSprites[level-1];
    }
    /// <summary>
    /// ������ �ش��ϴ� ��� �� �̹����� �����ɴϴ�. 
    /// </summary>
    /// <param name="level">1 ~ 9</param>
    /// <returns>Shield Sprite</returns>
    public Sprite GetShieldSprite(int level)
    {
        return shieldSprites[level];
    }
    
    /// <summary>
    /// ��� ���� ������ ����ϴ�.
    /// </summary>
    /// <param name="score">score</param>
    public void ScoreUp(int score)
    {
        stageScore += score;
        stageScore += score * combo;
        UIManager.Instance.SetScore(stageScore);
        //Debug.Log($"Score : {StageScore}");
    }

    /// <summary>
    /// ĳ���Ͱ� ���鿡�� ����� ��� �޺� Ƚ���� �ʱ�ȭ�մϴ�.
    /// </summary>
    public void GroundDefense()
    {
        //Debug.Log($"GroundDefense MaxCombo : {MaxCombo}, Combo : {Combo}");
        if (combo > maxCombo)
            maxCombo = combo;
        combo = 0;
    }

    /// <summary>
    /// ���� ���̺긦 �����մϴ�.
    /// ������ ���̺��ΰ�� ���� Ŭ����
    /// </summary>
    public void StartNextWave()
    {
        blockDestroyCount = 0;
        if (wave >= MAX_WAVE)
        {
            // Stage Clear
            GameClear();
            return;
        }
        BlockGenerate();
        wave++;
        ModulateWaveProgress();
        UIManager.Instance.SetWaveText("Wave" + wave.ToString("00"));
    }

    /// <summary>
    /// ���� Ŭ����
    /// </summary>
    void GameClear()
    {
        Debug.Log("StageClear");
        isClear = true;
        // ���� �� �� ��������
        if (currentStage > GameManager.Instance.GetClearStage())
        {
            stageCoin += 1000;
            stageDia += 10;
        }
        //GameManager.Instance.StageClear(CurrentStage);
        StartCoroutine(DelayShowResult());
    }

    /// <summary>
    /// ���ȭ���� �����ð� ���� �� �����ݴϴ�.
    /// </summary>
    /// <returns>WaitForSeconds(0.5f)</returns>
    IEnumerator DelayShowResult()
    {
        yield return new WaitForSeconds(0.5f);
        ShowResult();
    }

    /// <summary>
    /// ĳ������ ü���� ���� ���� ��� ���� ����
    /// </summary>
    void GameOver()
    {
        isClear = false;
        Debug.Log("GameOver");
        ShowResult();
    }

    /// <summary>
    /// �Ͻ�����
    /// </summary>
    public void PauseButtonOnClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupOpen);
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    /// <summary>
    /// ���� �̾��ϱ�
    /// </summary>
    public void RestartButtonOnClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupClose);
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// ������ �����ϰ� �κ�� �̵�
    /// </summary>
    public void ExitButtonOnClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupClose);
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        LoadingManager.LoadScene("Lobby", false);
    }

    /// <summary>
    /// ���� ����� �����ݴϴ�.
    /// </summary>
    void ShowResult()
    {
        isStart = false;
        Destroy(playerCharacter);
        for (int i = 0; i < MAX_BLOCK_COUNT; i++)
        {
            Destroy(blocks[i]);
            Destroy(blockController[i]);
        }


        stageCoin += currentStage * destroyCount;
        stageCoin += currentStage * maxCombo;
        if (extraCoin > 0)
            stageCoin += (int)(stageCoin * extraCoin);

        UIManager.Instance.SetResultTitle(isClear);
        UIManager.Instance.SetClearStage(currentStage);
        UIManager.Instance.SetResultScore(stageScore);
        UIManager.Instance.SetRewardCoin(stageCoin);
        UIManager.Instance.SetRewardDiamond(stageDia);
        UIManager.Instance.ShowResultView();

    }

    /// <summary>
    /// ���â���� Ȯ�� ��ư�� ���� ��� ȣ��˴ϴ�.
    /// ������ Ȯ���ϰ� �κ�� �̵��մϴ�.
    /// </summary>
    public void GetReward()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (!NetworkManager.Instance.GetIsConnected()) return;

        GameState state = isClear ? GameState.GameClear : GameState.GameOver;
        NetworkManager.Instance.GameEnd(state, currentStage, stageCoin, stageDia);


        //GameManager.Instance.AddCoin(StageCoin);
        //GameManager.Instance.AddDiamond(StageDia);
        LoadingManager.LoadScene("Lobby", false);
    }

    /// <summary>
    /// ���â���� ���� ���� ���� 3�� ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void GetAdvertisementReward()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (!NetworkManager.Instance.GetIsConnected()) return;

        GoogleMobileAdmobManager.Instance.ShowRewardedAd(OnAdCompleted);
    }

    /// <summary>
    /// ���� �� �Ŀ� ȣ��˴ϴ�.
    /// </summary>
    private void OnAdCompleted()
    {
        stageCoin *= REWARD_MULTIPLIER;
        stageDia *= REWARD_MULTIPLIER;
        GameState state = isClear ? GameState.GameClear : GameState.GameOver;
        NetworkManager.Instance.GameEnd(state, currentStage, stageCoin, stageDia);
        LoadingManager.LoadScene("Lobby", false);
    }

    public Vector2 GetPlayerCharacterPosition()
    {
        return playerCharacter.transform.position;
    }

    public float GetPlayerCharacterAltitude()
    {
        return playerCharacter.transform.position.y;
    }

    /// <summary>
    /// ĳ���Ͱ� ��Ͽ� �� ��� ȣ��˴ϴ�.
    /// ���� Ȯ���� �������� ���� �ʰ� ��Ƴ��ϴ�.
    /// �������� ���ȸ�� Ȯ���� ���Ե˴ϴ�.
    /// </summary>
    public void CharacterLifeDecrease()
    {
        // ���� ���
        if(Random.Range(0, 100) < absoluteDefense)
        {
            Debug.Log("���� ���!!!");

            // ĳ���ͺ��� ���� �̵��ϱ� ����
            Collider2D collider = playerCharacter.GetComponent<Collider2D>();
            float yPos = collider.bounds.size.y;

            Vector2 position = blockParent.transform.position;
            position.y += yPos;
            blockParent.transform.position = position;    // �ٴڿ��� ������ ��ġ�� �̵�

            CustomPhysics blockPhysics = blockParent.GetComponent<CustomPhysics>();
            blockPhysics.velocity = Vector2.up * 5f;

            absoluteDefenseParticle.ParticleActivation(collider.bounds.center); // ���� ��� ��ƼŬ
            return;
        }

        combo = 0;
        characterLife--;

        SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Hit);
        crashParticle.ParticleActivation(playerCharacter.transform.position);
        UIManager.Instance.LifeDecrease(characterLife);
        if (characterLife <= 0)
        {
            GameOver();
            return;
        }
    }

    /// <summary>
    /// ���� ���̺긦 Progress UI�� ǥ���մϴ�.
    /// </summary>
    public void ModulateWaveProgress()
    {
        float ratio = (float)blockDestroyCount / MAX_BLOCK_COUNT;
        float progress = wave - 1 + ratio;

        UIManager.Instance.SetWaveProgress(progress);
        // ���� Wave + wave ���൵
    }

    public bool GetIsClear()
    {
        return isClear;
    }

    /// <summary>
    /// ����� �缳���մϴ�.
    /// </summary>
    public void BlockGenerate()
    {
        targetBlockIndex = 0;

        CustomPhysics blockParentPhysics = blockParent.GetComponent<CustomPhysics>();
        blockParentPhysics.SetVelocity(Vector2.zero);
        blockParentPhysics.SetSimulated(false);

        Vector2 basePosition = GetPlayerCharacterPosition();
        basePosition.y += Camera.main.orthographicSize * 2;
        blockParent.transform.position = basePosition;

        Vector2 blockLocalPosition = Vector2.zero;
        for (int i = 0; i < MAX_BLOCK_COUNT; i++)
        {
            blockLocalPosition.y += 4;
            blocks[i].transform.localPosition = blockLocalPosition;

            int blockLevel = currentStage;
            int chance = Random.Range(0, 100);
            if (wave < HALF_WAVE)
            {
                if(currentStage > 1)
                {
                    // 0 ~ 25% Ȯ���� �� �ܰ� ���� ������ ����� ����
                    int variantProbability = (HALF_WAVE - wave) * BASE_PROBABILITY;
                    if (chance < variantProbability)
                    {
                        // ����
                        blockLevel--;
                    }
                }
            }
            else
            {
                if(currentStage < MAX_STAGE && currentStage > 1)
                {
                    // 0 ~ 25% Ȯ���� �� �ܰ� ���� ������ ����� ����
                    int variantProbability = (wave - HALF_WAVE + 1) * BASE_PROBABILITY;
                    if (chance < variantProbability)
                    {
                        // ����
                        blockLevel++;
                    }
                }
            }

            // ����� �� ����
            int shieldLevel = 0;
            // ���� �Ǵ� ���� ���� ���
            if (blockLevel == currentStage)
            {
                // �� ���δ� wave * 10% Ȯ���� ����
                if (Random.Range(0, 100) < (wave + 1) * 10)
                {
                    // Shield
                    // ���� ����� �� ������ 0~Wave �ܰ� �� �ϳ�
                    shieldLevel = Random.Range(0, wave + 1);
                }
            }
            else if(blockLevel < currentStage)
            {
                shieldLevel = Random.Range(0, MAX_SHIELD_LEVEL);
            }
            else
            {
                // < 2, 4, 6, 8, 10
                shieldLevel = Random.Range(0, (wave - HALF_WAVE + 1) * 2);
            }

            // ��� ü�� ����
            blockController[i].SetBlockHealthLevel(blockLevel);
            blockController[i].SetBlockShieldLevel(shieldLevel);
            blocks[i].SetActive(true);
        }

        blockParent.GetComponent<CustomPhysics>().SetSimulated(true);
        UIManager.Instance.SetBlockHealth(blockController[targetBlockIndex].GetBlockTotalCurrentHealth(), blockController[targetBlockIndex].GetBlockTotalMaxHealth());
        UIManager.Instance.SetBlockHealthActive(true);
    }

    /// <summary>
    /// ����� ü���� 0�̵Ǿ� �ı��� ��� ȣ��˴ϴ�.
    /// </summary>
    public void OnDestroyBlock()
    {
        //Debug.Log("DestroyBlock" + TargetBlockIndex.ToString());
        SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Destroy);

        // ���� Ȯ���� ���ʽ� ���̾Ƹ� ����ϴ�.
        if(Random.Range(0, 100) < BONUS_PROBABILITY)
        {
            stageDia++;
            SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.GetDia);
            diamondParticle.ParticleActivation(GetPlayerCharacterPosition());
        }

        targetBlockIndex++;
        blockDestroyCount++;
        destroyCount++;
        combo++;

        UIManager.Instance.ComboActivation(combo);
        ModulateWaveProgress();
        if (targetBlockIndex >= MAX_BLOCK_COUNT)
        {
            // ���� ��������
            UIManager.Instance.SetBlockHealthActive(false);
            StartNextWave();
        }

        if (!GetIsClear())
            UIManager.Instance.SetBlockHealth(blockController[targetBlockIndex].GetBlockTotalCurrentHealth(), blockController[targetBlockIndex].GetBlockTotalMaxHealth());

    }

    /// <summary>
    /// �ʱ� ����� �����մϴ�.
    /// </summary>
    private void CreateBlocks()
    {
        for (int i = 0; i < MAX_BLOCK_COUNT; i++)
        {
            blocks[i] = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity, blockParent.transform);
            blockController[i] = blocks[i].GetComponent<BlockController>();
            blockController[i].OnDestroyBlock += OnDestroyBlock;
            blocks[i].SetActive(false);
        }
    }
}
