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

    [SerializeField] Text text_Score;
    [SerializeField] GameObject[] life;

    [Header("Wave")]
    [SerializeField] Text text_Wave;
    [SerializeField] Slider waveProgress;

    [Header("BlockHealth")]
    [SerializeField] GameObject blockHealth;
    [SerializeField] Slider healthSlider;
    [SerializeField] Text healthText;

    [Header("DamageUI")]
    [SerializeField] DamageUI damageController;
    [SerializeField] GameObject text_Combo;


    [Header("Result")]
    [SerializeField] GameObject resultView;
    [SerializeField] GameObject btn_Claim;
    [SerializeField] GameObject btn_Reward;
    [SerializeField] Text text_Stage;
    [SerializeField] Text text_ResultTitle;
    [SerializeField] Text text_ResultScore;
    [SerializeField] Text text_ResultCoin;
    [SerializeField] Text text_ResultDia;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        SetScore(0);

        for(int i = 0; i < life.Length; i++)
        {
            life[i].SetActive(true);
        }

        resultView.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// 캐릭터의 체력이 감소되었을 경우 체력 UI 하나를 비활성화 합니다.
    /// </summary>
    /// <param name="lifeIndex">체력 UI 인덱스</param>
    public void LifeDecrease(int lifeIndex)
    {
        if (lifeIndex < 0) return;
        life[lifeIndex].SetActive(false);
    }

    /// <summary>
    /// 스코어를 표시합니다.
    /// </summary>
    /// <param name="score">Game Score</param>
    public void SetScore(int score)
    {
        text_Score.text = score.ToString();
    }

    /// <summary>
    /// 현재 웨이브를 표시합니다.
    /// </summary>
    /// <param name="str">Wave (1~10)</param>
    public void SetWaveText(string str)
    {
        text_Wave.text = str;
    }
    public void SetWaveProgress(float value)
    {
        if (value >= waveProgress.maxValue)
            value = waveProgress.maxValue;

        waveProgress.value = value;
    }

    public void SetBlockHealthActive(bool active)
    {
        blockHealth.SetActive(active);
    }

    public void SetBlockHealth(long currentHealth, long maxHealth)
    {
        float healthRatio = currentHealth / (float)maxHealth;
        healthSlider.value = healthRatio;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void ShowResultView()
    {
        resultView.SetActive(true);
        StartCoroutine(ButtonActiveDelay());
    }

    IEnumerator ButtonActiveDelay()
    {
        yield return new WaitForSeconds(1);
        btn_Claim.SetActive(true);
        btn_Reward.SetActive(true);
    }

    public void HideResultView()
    {
        btn_Claim.SetActive(false);
        btn_Reward.SetActive(false);
        resultView.SetActive(false);
    }

    public void SetResultTitle(bool isClear)
    {
        if (isClear)
        {
            text_ResultTitle.text = RESULT_CLEAR;
            SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.GameClear);
        }
        else
        {
            text_ResultTitle.text = RESULT_FAILED;
            SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.GameOver);
        }
    }

    public void SetClearStage(int stage)
    {
        text_Stage.text = stage.ToString();
    }

    public void SetResultScore(int score)
    {
        text_ResultScore.text = score.ToString();
    }

    public void SetRewardCoin(int coin)
    {
        text_ResultCoin.text = coin.ToString();
    }

    public void SetRewardDiamond(int dia)
    {
        text_ResultDia.text = dia.ToString();
    }

    public void ComboActivation(int combo)
    {
        text_Combo.GetComponent<TextMeshProUGUI>().text = combo.ToString() + "COMBO!";
        text_Combo.GetComponent<Animator>().Play("ComboAnimation", 0, 0f);
    }

    public void ShowDamage(Vector2 position, long damage)
    {
        //Debug.Log(position);
        damageController.ShowDamageUI(position, damage);
    }
}
