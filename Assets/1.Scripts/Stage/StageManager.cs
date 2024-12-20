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
    const int BONUS_PROBABILITY = 5;    // 보너스 확률

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

    public float cameraYOffset = 0f; // 카메라 기준 높이
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
        // 블록의 속도가 너무 빨라서 초반 진입장벽이 높음
        // 스테이지마다 서서히 빨라지게끔 조절
        // 중력 스케일 계산 (기본 속도 + 스테이지 추가 속도)
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
            // 캐릭터의 현재 위치
            Vector3 characterPosition = playerCharacter.transform.position;
            // 카메라의 현재 위치
            Vector3 cameraPosition = mainCamera.transform.position;

            // 캐릭터가 기준 높이(0)를 넘어선 경우 카메라가 추적
            if (characterPosition.y > cameraYOffset)
            {
                // 카메라의 y값을 캐릭터의 y값으로 업데이트
                cameraPosition.y = characterPosition.y;
            }
            else
            {
                // 카메라가 기준 높이 이하로 내려가지 않음
                cameraPosition.y = Mathf.Max(cameraYOffset, cameraPosition.y);
            }

            // 카메라 위치 업데이트
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
    /// 캐릭터가 지면에 착지한 후에 스테이지가 시작됩니다.
    /// </summary>
    IEnumerator GameStartDelay()
    {
        CustomPhysics physics = playerCharacter.GetComponent<CustomPhysics>();

        while(!physics.GetIsGround())
        {
            yield return null;
        }

        // 캐릭터가 지면에 도착하면 시작
        isStart = true;
        StartNextWave();
    }

    /// <summary>
    /// 레벨에 해당하는 블록 이미지를 가져옵니다.
    /// </summary>
    /// <param name="level">1~80</param>
    /// <returns>Block Sprite</returns>
    public Sprite GetBlockSprite(int level)
    {
        return blockSprites[level-1];
    }
    /// <summary>
    /// 레벨에 해당하는 블록 방어막 이미지를 가져옵니다. 
    /// </summary>
    /// <param name="level">1 ~ 9</param>
    /// <returns>Shield Sprite</returns>
    public Sprite GetShieldSprite(int level)
    {
        return shieldSprites[level];
    }
    
    /// <summary>
    /// 블록 격파 점수를 얻습니다.
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
    /// 캐릭터가 지면에서 방어할 경우 콤보 횟수를 초기화합니다.
    /// </summary>
    public void GroundDefense()
    {
        //Debug.Log($"GroundDefense MaxCombo : {MaxCombo}, Combo : {Combo}");
        if (combo > maxCombo)
            maxCombo = combo;
        combo = 0;
    }

    /// <summary>
    /// 다음 웨이브를 설정합니다.
    /// 마지막 웨이브인경우 게임 클리어
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
    /// 게임 클리어
    /// </summary>
    void GameClear()
    {
        Debug.Log("StageClear");
        isClear = true;
        // 아직 못 깬 스테이지
        if (currentStage > GameManager.Instance.GetClearStage())
        {
            stageCoin += 1000;
            stageDia += 10;
        }
        //GameManager.Instance.StageClear(CurrentStage);
        StartCoroutine(DelayShowResult());
    }

    /// <summary>
    /// 결과화면을 일정시간 지연 후 보여줍니다.
    /// </summary>
    /// <returns>WaitForSeconds(0.5f)</returns>
    IEnumerator DelayShowResult()
    {
        yield return new WaitForSeconds(0.5f);
        ShowResult();
    }

    /// <summary>
    /// 캐릭터의 체력이 남지 않은 경우 게임 오버
    /// </summary>
    void GameOver()
    {
        isClear = false;
        Debug.Log("GameOver");
        ShowResult();
    }

    /// <summary>
    /// 일시정지
    /// </summary>
    public void PauseButtonOnClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupOpen);
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    /// <summary>
    /// 게임 이어하기
    /// </summary>
    public void RestartButtonOnClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupClose);
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// 게임을 중지하고 로비로 이동
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
    /// 게임 결과를 보여줍니다.
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
    /// 결과창에서 확인 버튼을 누를 경우 호출됩니다.
    /// 보상을 확인하고 로비로 이동합니다.
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
    /// 결과창에서 광고 보고 보상 3배 버튼 클릭 시 호출됩니다.
    /// </summary>
    public void GetAdvertisementReward()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (!NetworkManager.Instance.GetIsConnected()) return;

        GoogleMobileAdmobManager.Instance.ShowRewardedAd(OnAdCompleted);
    }

    /// <summary>
    /// 광고를 본 후에 호출됩니다.
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
    /// 캐릭터가 블록에 깔린 경우 호출됩니다.
    /// 일정 확률로 데미지를 입지 않고 살아납니다.
    /// 아이템의 기사회생 확률이 포함됩니다.
    /// </summary>
    public void CharacterLifeDecrease()
    {
        // 절대 방어
        if(Random.Range(0, 100) < absoluteDefense)
        {
            Debug.Log("절대 방어!!!");

            // 캐릭터보다 위로 이동하기 위해
            Collider2D collider = playerCharacter.GetComponent<Collider2D>();
            float yPos = collider.bounds.size.y;

            Vector2 position = blockParent.transform.position;
            position.y += yPos;
            blockParent.transform.position = position;    // 바닥에서 떨어진 위치로 이동

            CustomPhysics blockPhysics = blockParent.GetComponent<CustomPhysics>();
            blockPhysics.velocity = Vector2.up * 5f;

            absoluteDefenseParticle.ParticleActivation(collider.bounds.center); // 절대 방어 파티클
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
    /// 현재 웨이브를 Progress UI에 표시합니다.
    /// </summary>
    public void ModulateWaveProgress()
    {
        float ratio = (float)blockDestroyCount / MAX_BLOCK_COUNT;
        float progress = wave - 1 + ratio;

        UIManager.Instance.SetWaveProgress(progress);
        // 현재 Wave + wave 진행도
    }

    public bool GetIsClear()
    {
        return isClear;
    }

    /// <summary>
    /// 블록을 재설정합니다.
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
                    // 0 ~ 25% 확률로 한 단계 낮은 레벨의 블록이 출현
                    int variantProbability = (HALF_WAVE - wave) * BASE_PROBABILITY;
                    if (chance < variantProbability)
                    {
                        // 변종
                        blockLevel--;
                    }
                }
            }
            else
            {
                if(currentStage < MAX_STAGE && currentStage > 1)
                {
                    // 0 ~ 25% 확률로 한 단계 높은 레벨의 블록이 출현
                    int variantProbability = (wave - HALF_WAVE + 1) * BASE_PROBABILITY;
                    if (chance < variantProbability)
                    {
                        // 변종
                        blockLevel++;
                    }
                }
            }

            // 블록의 방어막 설정
            int shieldLevel = 0;
            // 이전 또는 현재 레벨 블록
            if (blockLevel == currentStage)
            {
                // 방어막 여부는 wave * 10% 확률로 출현
                if (Random.Range(0, 100) < (wave + 1) * 10)
                {
                    // Shield
                    // 현재 블록의 방어막 수준은 0~Wave 단계 중 하나
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

            // 블록 체력 설정
            blockController[i].SetBlockHealthLevel(blockLevel);
            blockController[i].SetBlockShieldLevel(shieldLevel);
            blocks[i].SetActive(true);
        }

        blockParent.GetComponent<CustomPhysics>().SetSimulated(true);
        UIManager.Instance.SetBlockHealth(blockController[targetBlockIndex].GetBlockTotalCurrentHealth(), blockController[targetBlockIndex].GetBlockTotalMaxHealth());
        UIManager.Instance.SetBlockHealthActive(true);
    }

    /// <summary>
    /// 블록의 체력이 0이되어 파괴된 경우 호출됩니다.
    /// </summary>
    public void OnDestroyBlock()
    {
        //Debug.Log("DestroyBlock" + TargetBlockIndex.ToString());
        SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Destroy);

        // 일정 확률로 보너스 다이아를 얻습니다.
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
            // 다음 스테이지
            UIManager.Instance.SetBlockHealthActive(false);
            StartNextWave();
        }

        if (!GetIsClear())
            UIManager.Instance.SetBlockHealth(blockController[targetBlockIndex].GetBlockTotalCurrentHealth(), blockController[targetBlockIndex].GetBlockTotalMaxHealth());

    }

    /// <summary>
    /// 초기 블록을 생성합니다.
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
