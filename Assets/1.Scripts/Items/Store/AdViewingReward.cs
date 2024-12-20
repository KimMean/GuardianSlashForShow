using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdViewingReward : MonoBehaviour
{
    [SerializeField] string productID;
    private DateTime lastAdWatchTime;
    private readonly TimeSpan adCooldown = TimeSpan.FromMinutes(10); // 10�� ���� ����
    private const string LastAdWatchTimeKey = "LastAdWatchTime";

    private void Start()
    {
        LoadLastAdWatchTime();
    }

    /// <summary>
    /// ���� ��ư Ŭ��
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
            MessageManager.Instance.ShowMessage($"{remainingTime.Minutes}�� �� ���� �ٽ� �� �� �ֽ��ϴ�.");
        }
    }
    /// <summary>
    /// ������ ���� ��û�ð� ���� ���� �ð��� �������� Ȯ���մϴ�.
    /// </summary>
    private bool CanShowAd()
    {
        return DateTime.Now >= lastAdWatchTime + adCooldown;
    }

    /// <summary>
    /// ���� ��û�մϴ�.
    /// </summary>
    private void ShowRewardedAd()
    {
        GoogleMobileAdmobManager.Instance.ShowRewardedAd(OnAdCompleted);
    }

    /// <summary>
    /// ���� ��û �Ϸ� �� ȣ��˴ϴ�.
    /// </summary>
    private void OnAdCompleted()
    {
        // ���� ���� ���� ����
        NetworkManager.Instance.ItemPurchase(Packet.Payment.Local, productID);
    }

    /// <summary>
    /// ������ ���� ��û�ð��� �����մϴ�.
    /// </summary>
    private void SaveLastAdWatchTime()
    {
        PlayerPrefs.SetString(LastAdWatchTimeKey, lastAdWatchTime.ToString());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ���� ���������� ��û�� �ð��� �ҷ��ɴϴ�.
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
            lastAdWatchTime = DateTime.MinValue; // ��û ����� ���� ��� �ʱ�ȭ
        }
    }
}
