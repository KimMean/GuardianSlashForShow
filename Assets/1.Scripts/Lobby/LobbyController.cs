using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    [SerializeField] GameObject[] MainMenu;
    [SerializeField] StagePanelController _StagePanelController;

    [SerializeField] GameObject SettingPanel;

    [Header("TopMenu")]
    [SerializeField] Text Text_Title;
    [SerializeField] GameObject Text_Coin;
    [SerializeField] GameObject Text_Diamond;


    int currentMenu = 0;

    private void Awake()
    {
        foreach(GameObject obj in MainMenu)
        {
            obj.SetActive(false);
        }
        SettingPanel.SetActive(false);
        MainMenu[currentMenu].SetActive(true);
        Text_Title.text = MainMenu[currentMenu].name;
    }

    private void Start()
    {
        SoundManager.Instance.ChangeBGM(SoundManager.BGM_Clip.Lobby);
        OnCoinChanged();
        OnDiamondChanged();

    }
    private void OnEnable()
    {
        GameManager.Instance.OnCoinChanged += OnCoinChanged;
        GameManager.Instance.OnDiamondChanged += OnDiamondChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCoinChanged -= OnCoinChanged;
            GameManager.Instance.OnDiamondChanged -= OnDiamondChanged;
        }
    }
    private void OnDestroy()
    {
    }

    /// <summary>
    /// 메뉴 버튼 클릭
    /// </summary>
    /// <param name="index">홈 / 무기 / 목걸이 / 반지 / 상점</param>
    public void MenuButtonOnClick(int index)
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (currentMenu == index) return;

        MainMenu[currentMenu].SetActive(false);
        currentMenu = index;
        MainMenu[currentMenu].SetActive(true);
        Text_Title.text = MainMenu[currentMenu].name;

    }

    /// <summary>
    /// 스테이지 시작 버튼 클릭
    /// </summary>
    public void StageStartButtonOnClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (!NetworkManager.Instance.GetIsConnected()) return;

        int targetStage = _StagePanelController.GetTargetStage();
        int clearStage = GameManager.Instance.GetClearStage();

        Debug.Log($"TargetStage : {targetStage}, ClearStage : {clearStage}");
        if(targetStage > clearStage+1)
        {
            //  Can't Start
            return;
        }

        GameManager.Instance.SetCurrentStage(targetStage);
        // SceneLoad
        LoadingManager.LoadScene("Stage", false);
    }

    public void ShowSettingPanel()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupOpen);
        SettingPanel.SetActive(true);
    }

    
    public void OnCoinChanged()
    {
        //Text_Coin.text = GameManager.Instance.GetCoin().ToString();
        Text_Coin.GetComponent<CustomSizeFitter>().SetContent(GameManager.Instance.GetCoin().ToString());
    }

    public void OnDiamondChanged()
    {
        //Text_Diamond.text = GameManager.Instance.GetDiamond().ToString();
        Text_Diamond.GetComponent<CustomSizeFitter>().SetContent(GameManager.Instance.GetDiamond().ToString());
    }
}
