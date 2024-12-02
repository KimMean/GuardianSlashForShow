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


    public void MenuButtonOnClick(int index)
    {
        if (currentMenu == index) return;

        MainMenu[currentMenu].SetActive(false);
        currentMenu = index;
        MainMenu[currentMenu].SetActive(true);
        Text_Title.text = MainMenu[currentMenu].name;

        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
    }

    public void StageStartButtonOnClick()
    {
        int targetStage = _StagePanelController.GetTargetStage();
        int clearStage = GameManager.Instance.GetClearStage();

        Debug.Log($"TargetStage : {targetStage}, ClearStage : {clearStage}");
        if(targetStage > clearStage+1)
        {
            //  Can't Start
            return;
        }

        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        GameManager.Instance.SetCurrentStage(targetStage);
        // SceneLoad
        LoadingManager.LoadScene("Stage", false);
    }

    public void ShowSettingPanel()
    {
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
