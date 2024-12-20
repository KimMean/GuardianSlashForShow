using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdViewingReward : MonoBehaviour
{
    [SerializeField] string productID;
    private DateTime lastAdWatchTime;
    private readonly TimeSpan adCooldown = TimeSpan.FromMinutes(10); // 10분 간격 제한
    private const string LastAdWatchTimeKey = "LastAdWatchTime";

    private void Start()
    {
        LoadLastAdWatchTime();
    }

    /// <summary>
    /// 광고 버튼 클릭
    /// </summary>
    public void ShowAd()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        if (!NetworkManager.Instance.GetIsConnected()) return;

        if (CanShowAd())
        {
            ShowRewardedAd();
            lastAdWatchTime = DateTime.Now;
            SaveLastAdWatchTime();
        }
        else
        {
            TimeSpan remainingTime = (lastAdWatchTime + adCooldown) - DateTime.Now;
            MessageManager.Instance.ShowMessage($"{remainingTime.Minutes}분 후 광고를 다시 볼 수 있습니다.");
        }
    }
    /// <summary>
    /// 마지막 광고 시청시간 이후 일정 시간이 지났는지 확인합니다.
    /// </summary>
    private bool CanShowAd()
    {
        return DateTime.Now >= lastAdWatchTime + adCooldown;
    }

    /// <summary>
    /// 광고를 요청합니다.
    /// </summary>
    private void ShowRewardedAd()
    {
        GoogleMobileAdmobManager.Instance.ShowRewardedAd(OnAdCompleted);
    }

    /// <summary>
    /// 광고 시청 완료 시 호출됩니다.
    /// </summary>
    private void OnAdCompleted()
    {
        // 보상 지급 로직 구현
        NetworkManager.Instance.ItemPurchase(Packet.Payment.Local, productID);
    }

    /// <summary>
    /// 마지막 광고 시청시간을 저장합니다.
    /// </summary>
    private void SaveLastAdWatchTime()
    {
        PlayerPrefs.SetString(LastAdWatchTimeKey, lastAdWatchTime.ToString());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 광고를 마지막으로 시청한 시간을 불러옵니다.
    /// </summary>
    private void LoadLastAdWatchTime()
    {
        if (PlayerPrefs.HasKey(LastAdWatchTimeKey))
        {
            string savedTime = PlayerPrefs.GetString(LastAdWatchTimeKey);
            DateTime.TryParse(savedTime, out lastAdWatchTime);
        }
        else
        {
            lastAdWatchTime = DateTime.MinValue; // 시청 기록이 없는 경우 초기화
        }
    }
}
