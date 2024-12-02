using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance => instance;

    const string RESULT_CLEAR = "GAME CLEAR";
    const string RESULT_FAILED = "GAME OVER";

    [SerializeField] Text Text_Score;
    [SerializeField] GameObject[] Life;

    [Header("Wave")]
    [SerializeField] Text Text_Wave;
    [SerializeField] Slider WaveProgress;

    [Header("BlockHealth")]
    [SerializeField] GameObject BlockHealth;
    [SerializeField] Slider HealthSlider;
    [SerializeField] Text HealthText;

    [Header("DamageUI")]
    [SerializeField] DamageUI DamageController;
    [SerializeField] GameObject Text_Combo;


    [Header("Result")]
    [SerializeField] GameObject ResultView;
    [SerializeField] GameObject Btn_Claim;
    [SerializeField] GameObject Btn_Reward;
    [SerializeField] Text Text_ResultTitle;
    [SerializeField] Text Text_Stage;
    [SerializeField] Text Text_ResultScore;
    [SerializeField] Text Text_ResultCoin;
    [SerializeField] Text Text_ResultDia;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        SetScore(0);

        for(int i = 0; i < Life.Length; i++)
        {
            Life[i].SetActive(true);
        }

        ResultView.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LifeDecrease(int lifeIndex)
    {
        if (lifeIndex < 0) return;
        Life[lifeIndex].SetActive(false);
    }

    public void SetScore(int score)
    {
        Text_Score.text = score.ToString();
    }

    public void SetWaveText(string str)
    {
        Text_Wave.text = str;
    }
    public void SetWaveProgress(float value)
    {
        if (value >= WaveProgress.maxValue)
            value = WaveProgress.maxValue;

        WaveProgress.value = value;
    }

    public void SetBlockHealthActive(bool active)
    {
        BlockHealth.SetActive(active);
    }

    public void SetBlockHealth(long currentHealth, long maxHealth)
    {
        float healthRatio = currentHealth / (float)maxHealth;
        HealthSlider.value = healthRatio;
        HealthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void ShowResultView()
    {
        ResultView.SetActive(true);
        StartCoroutine(ButtonActiveDelay());
    }

    IEnumerator ButtonActiveDelay()
    {
        yield return new WaitForSeconds(1);
        Btn_Claim.SetActive(true);
        Btn_Reward.SetActive(true);
    }

    public void HideResultView()
    {
        Btn_Claim.SetActive(false);
        Btn_Reward.SetActive(false);
        ResultView.SetActive(false);
    }

    public void SetResultTitle(bool isClear)
    {
        if (isClear)
        {
            Text_ResultTitle.text = RESULT_CLEAR;
            SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.GameClear);
        }
        else
        {
            Text_ResultTitle.text = RESULT_FAILED;
            SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.GameOver);
        }
    }

    public void SetClearStage(int stage)
    {
        Text_Stage.text = stage.ToString();
    }

    public void SetResultScore(int score)
    {
        Text_ResultScore.text = score.ToString();
    }

    public void SetRewardCoin(int coin)
    {
        Text_ResultCoin.text = coin.ToString();
    }

    public void SetRewardDiamond(int dia)
    {
        Text_ResultDia.text = dia.ToString();
    }

    public void ComboActivation(int combo)
    {
        Text_Combo.GetComponent<TextMeshProUGUI>().text = combo.ToString() + "COMBO!";
        Text_Combo.GetComponent<Animator>().Play("ComboAnimation", 0, 0f);
    }

    public void ShowDamage(Vector2 position, long damage)
    {
        //Debug.Log(position);
        DamageController.ShowDamageUI(position, damage);
    }
}
