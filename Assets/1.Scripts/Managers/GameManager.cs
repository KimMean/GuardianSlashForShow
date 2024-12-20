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
    /// ���� ���� �˸�
    /// </summary>
    public event Action OnCoinChanged;
    /// <summary>
    /// ���̾Ƹ�� ���� �˸� 
    /// </summary>
    public event Action OnDiamondChanged;
    /// <summary>
    /// ���� ���� �˸�
    /// </summary>
    public event Action OnChangedEquippedWeapon;
    /// <summary>
    /// ����� ���� �˸�
    /// </summary>
    public event Action OnChangedEquippedNecklace;
    /// <summary>
    /// ���� ���� �˸�
    /// </summary>
    public event Action OnChangedEquippedRing;

    /// <summary>
    /// �α��� ȭ�鿡�� �ڷΰ��� �Ǵ� Esc�� ���� ��� �����˴ϴ�.
    /// quitPanel���� ������ �����ϴ� ����� ���Ե˴ϴ�.
    /// </summary>
    [SerializeField] GameObject quitPanelPrefab;
    /// <summary>
    /// quitPanel�� ������ ��� ����� �����մϴ�.
    /// �� ����� null���� �����ϴ�.
    /// </summary>
    GameObject quitPanel = null;

    /*
     * [Stage]
     * ���������� 1���� ������ (1 ~ 80)
     * Ŭ������ ���������� 0�� ��� �ƹ��͵� Ŭ�������� ���� ��.
     * ���������� Ŭ������ ������������ 1 ���� ���� ������ �� ����
     * ������� 1���������� Ŭ�����ϸ� 2���������� ������ ������
     */
    /// <summary>
    /// �ִ� ��������
    /// </summary>
    const int MAX_STAGE_COUNT = 80;
    /// <summary>
    /// ���� �������� ���������� ǥ���մϴ�.
    /// </summary>
    int currentStage = 0;
    /// <summary>
    /// Ŭ������ ���������� �ִ밪�� �����ϴ�.
    /// </summary>
    int clearStage = 0;     // �ִ� Ŭ������ ��������

    /*
     * [Item]
     * �������� �������� ǥ���մϴ�.
     * Weapon�� �⺻���� W001 �Դϴ�.
     * Necklace�� Ring�� �⺻ ���� 000���� �ƹ��͵� �������� ���� �����Դϴ�.
     */
    string equipWeaponCode = "W001";
    string equipNecklaceCode = "N000";
    string equipRingCode = "R000";

    /*
     * [wealth]
     */
    int Coin = 0;           // ������ �ִ� ��
    int Diamond = 0;        // ������ �ִ� ��ȭ

    /// <summary>
    /// ���� ������ �ʽ��ϴ�.
    /// ó������ ���� ��� ���� �ð�(24H)�� �ʰ��� ��� ������ ������� �ʽ��ϴ�.
    /// ���α׷��� �����ϰ� �� �α��� �� ���ٽð��� �ʱ�ȭ�˴ϴ�.
    /// </summary>
    DateTime accessTime; // ������ Ŭ�� �ð� ����

    /// <summary>
    /// ���� �� ������ ��Ÿ���ϴ�.
    /// </summary>
    string appVersion = "private";
    /// <summary>
    /// �� ������ �ٸ� ��� ����� ������� URL�Դϴ�.
    /// ������ ����� �� ������ ���� �� �ֽ��ϴ�.
    /// </summary>
    string storeUrl = "";
    /// <summary>
    /// ������Ʈ ���θ� ��Ÿ���ϴ�.
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
    /// Editor �󿡼� ������ ���Ḧ �����մϴ�.
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
    /// ���� �� ������ ������ �� ������ ���մϴ�.
    /// </summary>
    /// <param name="version">������ ����� �� ����</param>
    /// <param name="url">������ ����� ����� �ּ�</param>
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
    /// ���α׷��� ����ǰ� �ִ� �÷����� Ȯ���մϴ�.
    /// </summary>
    /// <returns>��Ÿ�� �÷���</returns>
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
    /// ������Ʈ�� �����ϴ� ��� True�� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>ExistUpdate</returns>
    public bool GetExistUpdate()
    {
        return existUpdate;
    }

    /// <summary>
    /// �÷����� ���� ������Ʈ�� �� �ִ� ����� �ּҸ� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>StoreURL</returns>
    public string GetStoreURL()
    {
        return storeUrl;
    }

    /// <summary>
    /// �α����� �����ϸ� ���� �ð��� ���� �ð����� �����մϴ�.
    /// </summary>
    public void SetAccessTime()
    {
        accessTime = DateTime.Now;
    }

    /// <summary>
    /// ���� ������ �ʽ��ϴ�.
    /// �ִ� ���� �ð��� 24�ð� �Դϴ�.
    /// ���� ���� �ð��� �ʰ��� ��� AccessToken�� ����ǹǷ� ó���� �ʿ��մϴ�.
    /// </summary>
    /// <returns>���� �ð� �ʰ� ����</returns>
    public bool IsAccessTimeOut()
    {
        bool isTimeOut = false;
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsed = currentTime - accessTime;

        if (elapsed.TotalHours >= 1 && elapsed.Seconds <= 10)
        {
            Debug.Log("�� �α����� �ʿ��մϴ�. ��� �ð�: " + elapsed.Seconds + " �ð�");
            isTimeOut = true;
            // ���⿡�� ��α��� ���� �߰� ����
        }

        return isTimeOut;
    }

    #region [Stage]
    /// <summary>
    /// �ִ� ���������� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>80</returns>
    public int GetMaxStageCount()
    {
        return MAX_STAGE_COUNT;
    }

    /// <summary>
    /// ���� ���������� �����մϴ�.
    /// </summary>
    /// <param name="stage">1 ~ 80</param>
    public void SetCurrentStage(int stage)
    {
        currentStage = stage;
    }
    /// <summary>
    /// ���� ���������� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>currentStage (1~80)</returns>
    public int GetCurrentStage()
    {
        return currentStage;
    }
    /// <summary>
    /// Ŭ������ �������� �� �ִ� ���� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>ClearStage (1~80)</returns>
    public int GetClearStage()
    {
        return clearStage;
    }
    /// <summary>
    /// Ŭ������ ���������� �����մϴ�.
    /// �α��ν� �����˴ϴ�.
    /// </summary>
    /// <param name="stage">ClearStage (1~80)</param>
    public void SetClearStage(int stage)
    {
        clearStage = stage;
    }

    /// <summary>
    /// ������ Ŭ������ ��� ȣ��˴ϴ�.
    /// ���� Ŭ������ ���������� ���Ͽ� �����մϴ�.
    /// </summary>
    /// <param name="stage">Stage Clear (1~80)</param>
    public void StageClear(int stage)
    {
        // �̹� Ŭ������ ��������
        if (stage >= clearStage)
        {
            clearStage = stage;
        }
    }
    #endregion

    #region [Eqipment]
    /// <summary>
    /// ���� �����ϰ� �ִ� ���� �ڵ带 ��ȯ�մϴ�.
    /// </summary>
    /// <returns>W001 ~ W020</returns>
    public string GetEquipmentWeapon()
    {
        return equipWeaponCode;
    }
    /// <summary>
    /// ������ ���� �ڵ带 �����մϴ�.
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
    /// ���� �������� ������ڵ带 ��ȯ�մϴ�.
    /// </summary>
    /// <returns>N000 ~ N015</returns>
    public string GetEquipmentNecklace()
    {
        return equipNecklaceCode;
    }
    /// <summary>
    /// ������ ����� �ڵ带 �����մϴ�.
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
    /// ���� �������� ���� �ڵ带 ��ȯ�մϴ�.
    /// </summary>
    /// <returns>R000 ~ R015</returns>
    public string GetEquipmentRing()
    {
        return equipRingCode;
    }

    /// <summary>
    /// ������ ���� �ڵ带 �����մϴ�.
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
    /// ���� ������ �ִ� ������ ��ȯ�մϴ�.
    /// </summary>
    /// <returns>Coin</returns>
    public int GetCoin()
    {
        return Coin;
    }
    /// <summary>
    /// ������ �缳���մϴ�.
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
    /// ���� �������ִ� ���̾Ƹ�带 ��ȯ�մϴ�.
    /// </summary>
    /// <returns>Diamond</returns>
    public int GetDiamond()
    {
        return Diamond;
    }
    /// <summary>
    /// ���̾Ƹ�带 �缳���մϴ�.
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
    /// ���� ����� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="scene">New Scene</param>
    /// <param name="mode">Load Scene Mode</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (quitPanel != null)
            quitPanel = null;
    }

    /// <summary>
    /// ���� ��ư�� ������ �� ȣ��˴ϴ�.
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
