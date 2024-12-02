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

    public event Action OnCoinChanged;
    public event Action OnDiamondChanged;
    public event Action OnChangedEquippedWeapon;
    public event Action OnChangedEquippedNecklace;
    public event Action OnChangedEquippedRing;

    /*
     * [Stage]
     * ���������� 1���� ������ (1 ~ 80)
     * Ŭ������ ���������� 0�� ��� �ƹ��͵� Ŭ�������� ���� ��.
     * ���������� Ŭ������ ������������ 1 ���� ���� ������ �� ����
     * ������� 1���������� Ŭ�����ϸ� 2���������� ������ ������
     */
    const int MAX_STAGE_COUNT = 80;
    int CurrentStage = 0;
    int ClearStage = 0;     // �ִ� Ŭ������ ��������

    /*
     * [Item]
     */
    string EquipWeaponCode = "W001";
    string EquipNecklaceCode = "N000";
    string EquipRingCode = "R000";

    /*
     * [wealth]
     */
    int Coin = 0;           // ������ �ִ� ��
    int Diamond = 0;        // ������ �ִ� ��ȭ

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

    // Update is called once per frame
    void Update()
    {
    }


    #region [Stage]
    public int GetMaxStageCount()
    {
        return MAX_STAGE_COUNT;
    }

    public void SetCurrentStage(int stage)
    {
        CurrentStage = stage;
    }
    public int GetCurrentStage()
    {
        return CurrentStage;
    }
    public int GetClearStage()
    {
        return ClearStage;
    }
    public void SetClearStage(int stage)
    {
        ClearStage = stage;
    }

    public void StageClear(int stage)
    {
        // �̹� Ŭ������ ��������
        if (stage >= ClearStage)
        {
            ClearStage = stage;
        }
    }
    #endregion

    #region [Eqipment]
    public string GetEquipmentWeapon()
    {
        return EquipWeaponCode;
    }
    public void SetEquipmentWeapon(string code)
    {
        EquipWeaponCode = code;
        if(OnChangedEquippedWeapon != null)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                OnChangedEquippedWeapon.Invoke();
                SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.EquipSword);
            });
        }
    }
    public string GetEquipmentNecklace()
    {
        return EquipNecklaceCode;
    }
    public void SetEquipmentNecklace(string code)
    {
        EquipNecklaceCode = code;
        if (OnChangedEquippedNecklace != null)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                OnChangedEquippedNecklace.Invoke();
                SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.EquipNecklace);
            });
        }
    }
    public string GetEquipmentRing()
    {
        return EquipRingCode;
    }
   
    public void SetEquipmentRing(string code)
    {
        EquipRingCode = code;
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
    public int GetCoin()
    {
        return Coin;
    }
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
    public void AddCoin(int coin)
    {
        Coin += coin;
        if (OnCoinChanged != null)
            MainThreadDispatcher.Instance.Enqueue(OnCoinChanged.Invoke);
    }

    public void SubtractCoin(int coin)
    {
        Coin -= coin;
        if (OnCoinChanged != null)
            MainThreadDispatcher.Instance.Enqueue(OnCoinChanged.Invoke);
    }
    public int GetDiamond()
    {
        return Diamond;
    }
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
    public void AddDiamond(int diamond)
    {
        Diamond += diamond;
        if (OnDiamondChanged != null)
            MainThreadDispatcher.Instance.Enqueue(OnDiamondChanged.Invoke);
    }
    public void SubtractDiamond(int diamond)
    {
        Diamond -= diamond;
        if (OnDiamondChanged != null)
            MainThreadDispatcher.Instance.Enqueue(OnDiamondChanged.Invoke);
    }
    #endregion


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Scene " + scene.name + "loaded with mode " + mode);
        if(scene.name.Equals("Stage_Forest"))
        {
            //Debug.Log("Forest Loaded");
            OnStageLoaded();
        }
        else
        {
            Debug.Log(scene.name + "Loaded");
        }
    }

    private void OnStageLoaded()
    {
        Debug.Log("StageLoaded");
    }


}
