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

    [SerializeField] GameObject PlayerCharacterPrefab;
    GameObject PlayerCharacter;
    CharacterController characterController;

    [SerializeField] GameObject BlockParent;
    [SerializeField] GameObject BlockPrefab;
    Sprite[] BlockSprites = new Sprite[MAX_STAGE];
    Sprite[] ShieldSprites = new Sprite[MAX_SHIELD_COUNT];
    GameObject[] Blocks;
    BlockController[] _BlockController;
    [SerializeField] ParticleManager crashParticle;
    [SerializeField] ParticleManager absoluteDefenseParticle;
    [SerializeField] ParticleManager diamondParticle;

    float CameraOffset = 0;
    public float cameraYOffset = 0f; // 카메라 기준 높이
    bool IsStart = false;

    int TargetBlockIndex = 0;

    int CharacterLife;

    int CurrentStage = 0;
    int Wave = 0;
    bool IsClear;

    int BlockDestroyCount = 0;
    int DestroyCount = 0;

    int StageScore = 0;
    int StageCoin = 0;
    int StageDia = 0;
    int Combo = 0;
    int MaxCombo = 0;

    float ExtraCoin = 0;
    float Deceleration = 0;
    float AbsoluteDefense = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        mainCamera = Camera.main;

        PlayerCharacter = Instantiate(PlayerCharacterPrefab, Vector3.zero, Quaternion.identity);
        characterController = PlayerCharacter.GetComponent<CharacterController>();

        CurrentStage = GameManager.Instance.GetCurrentStage();

        for (int i = 1; i <= MAX_STAGE; i++)
        {
            BlockSprites[i - 1] = Resources.Load<Sprite>("Blocks/Stage" + i.ToString("00"));
        }
        for (int i = 1; i <= MAX_SHIELD_COUNT; i++)
        {
            ShieldSprites[i - 1] = Resources.Load<Sprite>("Shield/Shield" + i.ToString());
            //Debug.Log(ShieldSprites[i - 1]);
        }

        Blocks = new GameObject[MAX_BLOCK_COUNT];
        _BlockController = new BlockController[MAX_BLOCK_COUNT];

        string ringCode = GameManager.Instance.GetEquipmentRing();
        if (ringCode != "R000")
            ExtraCoin = RingManager.Instance.GetRingData(ringCode).GetGold() / 100f;

        string necklaceCode = GameManager.Instance.GetEquipmentNecklace();
        if(necklaceCode != "N000")
        {
            AbsoluteDefense = NecklaceManager.Instance.GetNecklaceData(necklaceCode).GetTwilight();
        }
    }

    private void OnEnable()
    {
        UIManager.Instance.SetBlockHealthActive(false);

        IsClear = false;
        Wave = 0;
        CharacterLife = 3;
        StageScore = 0;
        BlockDestroyCount = 0;
        DestroyCount = 0;
        StageCoin = 0;
        StageDia = 0;
        Combo = 0;
        MaxCombo = 0;

        CreateBlocks();
    }
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(StageStartDelay());
        StartCoroutine(GameStartDelay());
        SoundManager.Instance.ChangeBGM(SoundManager.BGM_Clip.Game);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsStart)
        {
            // 캐릭터의 현재 위치
            Vector3 characterPosition = PlayerCharacter.transform.position;
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

    //IEnumerator StageStartDelay()
    //{
    //    yield return new WaitForSeconds(1f);
    //    StartNextWave();
    //}
    
    IEnumerator GameStartDelay()
    {
        CustomPhysics physics = PlayerCharacter.GetComponent<CustomPhysics>();

        while(!physics.GetIsGround())
        {
            yield return null;
        }

        //CameraOffset = mainCamera.transform.position.y - PlayerCharacter.transform.position.y;
        //Debug.Log($"CharacterOffset : {CameraOffset}");

        // 캐릭터가 지면에 도착하면 시작
        IsStart = true;
        StartNextWave();
    }

    public Sprite GetBlockSprite(int level)
    {
        return BlockSprites[level-1];
    }
    public Sprite GetShieldSprite(int level)
    {
        return ShieldSprites[level];
    }

    public void ScoreUp(int score)
    {
        StageScore += score;
        StageScore += score * Combo;
        UIManager.Instance.SetScore(StageScore);
        //Debug.Log($"Score : {StageScore}");
    }

    public void GroundDefense()
    {
        //Debug.Log($"GroundDefense MaxCombo : {MaxCombo}, Combo : {Combo}");
        if (Combo > MaxCombo)
            MaxCombo = Combo;
        Combo = 0;
    }

    public void StartNextWave()
    {
        BlockDestroyCount = 0;
        if (Wave >= MAX_WAVE)
        {
            // Stage Clear
            GameClear();
            return;
        }
        BlockGenerate();
        Wave++;
        ModulateWaveProgress();
        UIManager.Instance.SetWaveText("Wave" + Wave.ToString("00"));
    }

    void GameClear()
    {
        Debug.Log("StageClear");
        IsClear = true;
        // 아직 못 깬 스테이지
        if (CurrentStage > GameManager.Instance.GetClearStage())
        {
            StageCoin += 1000;
            StageDia += 10;
        }
        //GameManager.Instance.StageClear(CurrentStage);
        ShowResult();
    }

    void GameOver()
    {
        IsClear = false;
        Debug.Log("GameOver");
        ShowResult();
    }

    void ShowResult()
    {
        IsStart = false;
        Destroy(PlayerCharacter);
        for (int i = 0; i < MAX_BLOCK_COUNT; i++)
        {
            Destroy(Blocks[i]);
            Destroy(_BlockController[i]);
        }


        StageCoin += CurrentStage * DestroyCount;
        StageCoin += CurrentStage * MaxCombo;
        if (ExtraCoin > 0)
            StageCoin += (int)(StageCoin * ExtraCoin);

        UIManager.Instance.SetResultTitle(IsClear);
        UIManager.Instance.SetClearStage(CurrentStage);
        UIManager.Instance.SetResultScore(StageScore);
        UIManager.Instance.SetRewardCoin(StageCoin);
        UIManager.Instance.SetRewardDiamond(StageDia);
        UIManager.Instance.ShowResultView();

    }

    public void GetReward()
    {
        GameState state = IsClear ? GameState.GameClear : GameState.GameOver;
        NetworkManager.Instance.GameEnd(state, CurrentStage, StageCoin, StageDia);

        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        //GameManager.Instance.AddCoin(StageCoin);
        //GameManager.Instance.AddDiamond(StageDia);
        LoadingManager.LoadScene("Lobby", false);
    }

    public void GetAdvertisementReward()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        GoogleMobileAdmobManager.Instance.ShowRewardedAd(OnAdCompleted);
    }

    private void OnAdCompleted()
    {
        StageCoin *= REWARD_MULTIPLIER;
        StageDia *= REWARD_MULTIPLIER;
        GameState state = IsClear ? GameState.GameClear : GameState.GameOver;
        NetworkManager.Instance.GameEnd(state, CurrentStage, StageCoin, StageDia);
        LoadingManager.LoadScene("Lobby", false);
    }

    public Vector2 GetPlayerCharacterPosition()
    {
        return PlayerCharacter.transform.position;
    }

    public float GetPlayerCharacterAltitude()
    {
        return PlayerCharacter.transform.position.y;
    }

    public void CharacterLifeDecrease()
    {
        // 절대 방어
        if(Random.Range(0, 100) < AbsoluteDefense)
        {
            Debug.Log("절대 방어!!!");

            // 캐릭터보다 위로 이동하기 위해
            Collider2D collider = PlayerCharacter.GetComponent<Collider2D>();
            float yPos = collider.bounds.size.y;

            Vector2 position = BlockParent.transform.position;
            position.y += yPos;
            BlockParent.transform.position = position;    // 바닥에서 떨어진 위치로 이동

            CustomPhysics blockPhysics = BlockParent.GetComponent<CustomPhysics>();
            blockPhysics.Velocity = Vector2.up * 5f;

            absoluteDefenseParticle.ParticleActivation(collider.bounds.center); // 절대 방어 파티클
            return;
        }

        Combo = 0;
        CharacterLife--;

        SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Hit);
        crashParticle.ParticleActivation(PlayerCharacter.transform.position);
        UIManager.Instance.LifeDecrease(CharacterLife);
        if (CharacterLife <= 0)
        {
            GameOver();
            return;
        }
    }

    public void ModulateWaveProgress()
    {
        float ratio = (float)BlockDestroyCount / MAX_BLOCK_COUNT;
        float progress = Wave - 1 + ratio;

        UIManager.Instance.SetWaveProgress(progress);
        // 현재 Wave + wave 진행도
    }

    public bool GetIsClear()
    {
        return IsClear;
    }
    public void BlockGenerate()
    {
        TargetBlockIndex = 0;

        CustomPhysics blockParentPhysics = BlockParent.GetComponent<CustomPhysics>();
        blockParentPhysics.SetVelocity(Vector2.zero);
        blockParentPhysics.SetSimulated(false);

        Vector2 basePosition = GetPlayerCharacterPosition();
        basePosition.y += Camera.main.orthographicSize * 2;
        BlockParent.transform.position = basePosition;

        Vector2 blockLocalPosition = Vector2.zero;
        for (int i = 0; i < MAX_BLOCK_COUNT; i++)
        {
            blockLocalPosition.y += 4;
            Blocks[i].transform.localPosition = blockLocalPosition;

            int blockLevel = CurrentStage;
            int chance = Random.Range(0, 100);
            if (Wave < HALF_WAVE)
            {
                if(CurrentStage > 1)
                {
                    // 0 ~ 25% 확률로 한 단계 낮은 레벨의 블록이 출현
                    int variantProbability = (HALF_WAVE - Wave) * BASE_PROBABILITY;
                    if (chance < variantProbability)
                    {
                        // 변종
                        blockLevel--;
                    }
                }
            }
            else
            {
                if(CurrentStage < MAX_STAGE)
                {
                    // 0 ~ 25% 확률로 한 단계 높은 레벨의 블록이 출현
                    int variantProbability = (Wave - HALF_WAVE + 1) * BASE_PROBABILITY;
                    if (chance < variantProbability)
                    {
                        // 변종
                        blockLevel++;
                    }
                }
            }

            int shieldLevel = 0;
            // 이전 또는 현재 레벨 블록
            if (blockLevel == CurrentStage)
            {
                // 방어막 여부는 wave * 10% 확률로 출현
                if (Random.Range(0, 100) < (Wave+1) * 10)
                {
                    // Shield
                    // 현재 블록의 방어막 수준은 0~Wave 단계 중 하나
                    shieldLevel = Random.Range(0, Wave + 1);
                }
            }
            else if(blockLevel < CurrentStage)
            {
                shieldLevel = Random.Range(0, MAX_SHIELD_LEVEL);
            }
            else
            {
                // < 2, 4, 6, 8, 10
                shieldLevel = Random.Range(0, (Wave - HALF_WAVE + 1) * 2);
            }

            // 블록 체력 설정
            _BlockController[i].SetBlockHealthLevel(blockLevel);
            _BlockController[i].SetBlockShieldLevel(shieldLevel);
            Blocks[i].SetActive(true);
        }

        BlockParent.GetComponent<CustomPhysics>().SetSimulated(true);
        UIManager.Instance.SetBlockHealth(_BlockController[TargetBlockIndex].GetBlockTotalCurrentHealth(), _BlockController[TargetBlockIndex].GetBlockTotalMaxHealth());
        UIManager.Instance.SetBlockHealthActive(true);
    }

    public void OnDestroyBlock()
    {
        //Debug.Log("DestroyBlock" + TargetBlockIndex.ToString());
        SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Destroy);

        // 보너스 다이아
        if(Random.Range(0, 100) < BONUS_PROBABILITY)
        {
            StageDia++;
            SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.GetDia);
            diamondParticle.ParticleActivation(GetPlayerCharacterPosition());
        }

        TargetBlockIndex++;
        BlockDestroyCount++;
        DestroyCount++;
        // 콤보는 캐릭터가 바닥에서 막지 않아야 함
        Combo++;
        UIManager.Instance.ComboActivation(Combo);
        ModulateWaveProgress();
        if (TargetBlockIndex >= MAX_BLOCK_COUNT)
        {
            // 다음 스테이지
            UIManager.Instance.SetBlockHealthActive(false);
            StartNextWave();
        }

        if (!GetIsClear())
            UIManager.Instance.SetBlockHealth(_BlockController[TargetBlockIndex].GetBlockTotalCurrentHealth(), _BlockController[TargetBlockIndex].GetBlockTotalMaxHealth());

    }

    private void CreateBlocks()
    {
        for (int i = 0; i < MAX_BLOCK_COUNT; i++)
        {
            Blocks[i] = Instantiate(BlockPrefab, Vector3.zero, Quaternion.identity, BlockParent.transform);
            _BlockController[i] = Blocks[i].GetComponent<BlockController>();
            _BlockController[i].OnDestroyBlock += OnDestroyBlock;
            Blocks[i].SetActive(false);
        }
    }
}
