using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdViewingReward : MonoBehaviour
{
    [SerializeField] string productID;
    private DateTime lastAdWatchTime;
    private readonly TimeSpan adCooldown = TimeSpan.FromHours(1); // 1�ð� ���� ����
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
            MessageBox.GetComponent<MessageBox>().ShowMessage($"{remainingTime.Minutes}�� �� ���� �ٽ� �� �� �ֽ��ϴ�.");
            //Debug.Log($"{remainingTime.Minutes}�� �� ���� �ٽ� �� �� �ֽ��ϴ�. ");
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
        // ���� ���� ���� ����
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
            lastAdWatchTime = DateTime.MinValue; // ��û ����� ���� ��� �ʱ�ȭ
        }
    }
}
