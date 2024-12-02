using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdViewingReward : MonoBehaviour
{
    [SerializeField] string productID;
    private DateTime lastAdWatchTime;
    private readonly TimeSpan adCooldown = TimeSpan.FromHours(1); // 1시간 간격 제한
    private const string LastAdWatchTimeKey = "LastAdWatchTime";

    [SerializeField] GameObject MessageBox;

    private void Start()
    {
        LoadLastAdWatchTime();
    }

    public void ShowAd()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        if (CanShowAd())
        {
            ShowRewardedAd();
            lastAdWatchTime = DateTime.Now;
            SaveLastAdWatchTime();
        }
        else
        {
            TimeSpan remainingTime = (lastAdWatchTime + adCooldown) - DateTime.Now;
            MessageBox.GetComponent<MessageBox>().ShowMessage($"{remainingTime.Minutes}분 후 광고를 다시 볼 수 있습니다.");
            //Debug.Log($"{remainingTime.Minutes}분 후 광고를 다시 볼 수 있습니다. ");
        }
    }

    private bool CanShowAd()
    {
        return DateTime.Now >= lastAdWatchTime + adCooldown;
    }

    private void ShowRewardedAd()
    {
        GoogleMobileAdmobManager.Instance.ShowRewardedAd(OnAdCompleted);
    }

    private void OnAdCompleted()
    {
        // 보상 지급 로직 구현
        NetworkManager.Instance.ItemPurchase(Packet.Payment.Local, productID);
    }

    private void SaveLastAdWatchTime()
    {
        PlayerPrefs.SetString(LastAdWatchTimeKey, lastAdWatchTime.ToString());
        PlayerPrefs.Save();
    }

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
