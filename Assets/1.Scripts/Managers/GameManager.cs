using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    /// <summary>
    /// 코인 변경 알림
    /// </summary>
    public event Action OnCoinChanged;
    /// <summary>
    /// 다이아몬드 변경 알림 
    /// </summary>
    public event Action OnDiamondChanged;
    /// <summary>
    /// 무기 변경 알림
    /// </summary>
    public event Action OnChangedEquippedWeapon;
    /// <summary>
    /// 목걸이 변경 알림
    /// </summary>
    public event Action OnChangedEquippedNecklace;
    /// <summary>
    /// 반지 변경 알림
    /// </summary>
    public event Action OnChangedEquippedRing;

    /// <summary>
    /// 로그인 화면에서 뒤로가기 또는 Esc를 누를 경우 생성됩니다.
    /// quitPanel에는 게임을 종료하는 기능이 포함됩니다.
    /// </summary>
    [SerializeField] GameObject quitPanelPrefab;
    /// <summary>
    /// quitPanel이 생성된 경우 대상을 참조합니다.
    /// 씬 변경시 null값을 가집니다.
    /// </summary>
    GameObject quitPanel = null;

    /*
     * [Stage]
     * 스테이지는 1부터 시작함 (1 ~ 80)
     * 클리어한 스테이지는 0일 경우 아무것도 클리어하지 않은 것.
     * 스테이지는 클리어한 스테이지보다 1 높은 것을 도전할 수 있음
     * 예를들어 1스테이지를 클리어하면 2스테이지에 도전이 가능함
     */
    /// <summary>
    /// 최대 스테이지
    /// </summary>
    const int MAX_STAGE_COUNT = 80;
    /// <summary>
    /// 현재 도전중인 스테이지를 표시합니다.
    /// </summary>
    int currentStage = 0;
    /// <summary>
    /// 클리어한 스테이지의 최대값을 가집니다.
    /// </summary>
    int clearStage = 0;     // 최대 클리어한 스테이지

    /*
     * [Item]
     * 장착중인 아이템을 표시합니다.
     * Weapon의 기본값은 W001 입니다.
     * Necklace와 Ring의 기본 값은 000으로 아무것도 장착하지 않은 상태입니다.
     */
    string equipWeaponCode = "W001";
    string equipNecklaceCode = "N000";
    string equipRingCode = "R000";

    /*
     * [wealth]
     */
    int Coin = 0;           // 가지고 있는 돈
    int Diamond = 0;        // 가지고 있는 재화

    /// <summary>
    /// 현재 사용되지 않습니다.
    /// 처리하지 않을 경우 접근 시간(24H)이 초과된 경우 서버와 연결되지 않습니다.
    /// 프로그램을 종료하고 재 로그인 시 접근시간이 초기화됩니다.
    /// </summary>
    DateTime accessTime; // 마지막 클릭 시간 저장

    /// <summary>
    /// 현재 앱 버전을 나타냅니다.
    /// </summary>
    string appVersion = "private";
    /// <summary>
    /// 앱 버전이 다를 경우 연결될 스토어의 URL입니다.
    /// 서버와 연결된 후 정보를 받을 수 있습니다.
    /// </summary>
    string storeUrl = "";
    /// <summary>
    /// 업데이트 여부를 나타냅니다.
    /// </summary>
    bool existUpdate = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        //Debug.Log(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Editor 상에서 간편한 종료를 지원합니다.
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if(SceneManager.GetActiveScene().name == "Login")
            {
                if(quitPanel == null)
                {
                    quitPanel = Instantiate(quitPanelPrefab, FindObjectOfType<Canvas>().transform);
                }
                quitPanel.SetActive(true);
            }
            else if (SceneManager.GetActiveScene().name == "Lobby")
            {
                FindObjectOfType<LobbyController>().ShowSettingPanel();
            }
            else if (SceneManager.GetActiveScene().name == "Stage")
            {
                FindObjectOfType<StageManager>().PauseButtonOnClick();
            }
        }
    }

    /// <summary>
    /// 현재 앱 버전을 서버의 앱 버전과 비교합니다.
    /// </summary>
    /// <param name="version">서버에 저장된 앱 버전</param>
    /// <param name="url">서버에 저장된 스토어 주소</param>
    public void SetAppInformation(string version, string url)
    {
        if(appVersion != version)
        {
            existUpdate = true;
            storeUrl = url;
        }
        Debug.Log($"App Version : {version}, Store URL : {url}");
        Debug.Log($"Current App Version : {appVersion}");
    }

    /// <summary>
    /// 프로그램이 실행되고 있는 플랫폼을 확인합니다.
    /// </summary>
    /// <returns>런타임 플랫폼</returns>
    public string GetRuntimePlatform()
    {
        string platform;
        if (Application.platform == RuntimePlatform.WindowsEditor)
            platform = "Editor";
        else if (Application.platform == RuntimePlatform.Android)
            platform = "Android";
        else
            platform = "Unknown";

        return platform;
    }

    /// <summary>
    /// 업데이트가 존재하는 경우 True를 반환합니다.
    /// </summary>
    /// <returns>ExistUpdate</returns>
    public bool GetExistUpdate()
    {
        return existUpdate;
    }

    /// <summary>
    /// 플랫폼에 따라 업데이트할 수 있는 스토어 주소를 반환합니다.
    /// </summary>
    /// <returns>StoreURL</returns>
    public string GetStoreURL()
    {
        return storeUrl;
    }

    /// <summary>
    /// 로그인이 성공하면 접근 시간을 현재 시간으로 설정합니다.
    /// </summary>
    public void SetAccessTime()
    {
        accessTime = DateTime.Now;
    }

    /// <summary>
    /// 현재 사용되지 않습니다.
    /// 최대 접근 시간은 24시간 입니다.
    /// 접근 가능 시간을 초과한 경우 AccessToken이 만료되므로 처리가 필요합니다.
    /// </summary>
    /// <returns>접근 시간 초과 여부</returns>
    public bool IsAccessTimeOut()
    {
        bool isTimeOut = false;
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsed = currentTime - accessTime;

        if (elapsed.TotalHours >= 1 && elapsed.Seconds <= 10)
        {
            Debug.Log("재 로그인이 필요합니다. 경과 시간: " + elapsed.Seconds + " 시간");
            isTimeOut = true;
            // 여기에서 재로그인 로직 추가 가능
        }

        return isTimeOut;
    }

    #region [Stage]
    /// <summary>
    /// 최대 스테이지를 반환합니다.
    /// </summary>
    /// <returns>80</returns>
    public int GetMaxStageCount()
    {
        return MAX_STAGE_COUNT;
    }

    /// <summary>
    /// 현재 스테이지를 설정합니다.
    /// </summary>
    /// <param name="stage">1 ~ 80</param>
    public void SetCurrentStage(int stage)
    {
        currentStage = stage;
    }
    /// <summary>
    /// 현재 스테이지를 반환합니다.
    /// </summary>
    /// <returns>currentStage (1~80)</returns>
    public int GetCurrentStage()
    {
        return currentStage;
    }
    /// <summary>
    /// 클리어한 스테이지 중 최대 값을 반환합니다.
    /// </summary>
    /// <returns>ClearStage (1~80)</returns>
    public int GetClearStage()
    {
        return clearStage;
    }
    /// <summary>
    /// 클리어한 스테이지를 설정합니다.
    /// 로그인시 설정됩니다.
    /// </summary>
    /// <param name="stage">ClearStage (1~80)</param>
    public void SetClearStage(int stage)
    {
        clearStage = stage;
    }

    /// <summary>
    /// 게임을 클리어한 경우 호출됩니다.
    /// 현재 클리어한 스테이지와 비교하여 설정합니다.
    /// </summary>
    /// <param name="stage">Stage Clear (1~80)</param>
    public void StageClear(int stage)
    {
        // 이미 클리어한 스테이지
        if (stage >= clearStage)
        {
            clearStage = stage;
        }
    }
    #endregion

    #region [Eqipment]
    /// <summary>
    /// 현재 장착하고 있는 무기 코드를 반환합니다.
    /// </summary>
    /// <returns>W001 ~ W020</returns>
    public string GetEquipmentWeapon()
    {
        return equipWeaponCode;
    }
    /// <summary>
    /// 장착한 무기 코드를 변경합니다.
    /// </summary>
    /// <param name="code">W001 ~ W020</param>
    public void SetEquipmentWeapon(string code)
    {
        equipWeaponCode = code;
        if(OnChangedEquippedWeapon != null)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                OnChangedEquippedWeapon.Invoke();
                SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.EquipSword);
            });
        }
    }
    /// <summary>
    /// 현재 장착중인 목걸이코드를 반환합니다.
    /// </summary>
    /// <returns>N000 ~ N015</returns>
    public string GetEquipmentNecklace()
    {
        return equipNecklaceCode;
    }
    /// <summary>
    /// 장착한 목걸이 코드를 변경합니다.
    /// </summary>
    /// <param name="code">N000 ~ N015</param>
    public void SetEquipmentNecklace(string code)
    {
        equipNecklaceCode = code;
        if (OnChangedEquippedNecklace != null)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                OnChangedEquippedNecklace.Invoke();
                SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.EquipNecklace);
            });
        }
    }
    /// <summary>
    /// 현재 장착중인 반지 코드를 반환합니다.
    /// </summary>
    /// <returns>R000 ~ R015</returns>
    public string GetEquipmentRing()
    {
        return equipRingCode;
    }

    /// <summary>
    /// 장착한 반지 코드를 변경합니다.
    /// </summary>
    /// <param name="code">R000 ~ R015</param>
    public void SetEquipmentRing(string code)
    {
        equipRingCode = code;
        if (OnChangedEquippedRing != null)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                OnChangedEquippedRing.Invoke();
                SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.EquipRing);
            });
        }
    }

    #endregion

    #region [wealth]
    /// <summary>
    /// 현재 가지고 있는 코인을 반환합니다.
    /// </summary>
    /// <returns>Coin</returns>
    public int GetCoin()
    {
        return Coin;
    }
    /// <summary>
    /// 코인을 재설정합니다.
    /// </summary>
    /// <param name="coin">new Coin</param>
    public void SetCoin(int coin)
    {
        Coin = coin;
        if (OnCoinChanged != null)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                OnCoinChanged.Invoke();
                SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.GetCoin);
            });
        }
    }
    /// <summary>
    /// 현재 가지고있는 다이아몬드를 반환합니다.
    /// </summary>
    /// <returns>Diamond</returns>
    public int GetDiamond()
    {
        return Diamond;
    }
    /// <summary>
    /// 다이아몬드를 재설정합니다.
    /// </summary>
    /// <param name="diamond">New Diamond</param>
    public void SetDiamond(int diamond)
    {
        Diamond = diamond;
        if (OnDiamondChanged != null)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                OnDiamondChanged.Invoke();
                SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.GetDia);
            });
        }

    }
    #endregion

    /// <summary>
    /// 씬이 변경될 때 호출됩니다.
    /// </summary>
    /// <param name="scene">New Scene</param>
    /// <param name="mode">Load Scene Mode</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (quitPanel != null)
            quitPanel = null;
    }

    /// <summary>
    /// 종료 버튼을 눌렀을 때 호출됩니다.
    /// </summary>
    public void OnQuitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        Application.Quit();
#endif
    }

}
