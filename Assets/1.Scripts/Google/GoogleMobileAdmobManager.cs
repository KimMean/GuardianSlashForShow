using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleMobileAdmobManager : MonoBehaviour
{
    private static GoogleMobileAdmobManager instance;
    public static GoogleMobileAdmobManager Instance
    {
        get
        { 
            return instance; 
        }
    }

    const string REWARD_ADS_TEST_UNIT_ID =  "ca-app-pub-3940256099942544/5224354917";
    const string REWARD_ADS_UNIT_ID = "ca-app-pub-6690936278194276/3952341079";
    const string Interstitial_ADS_TEST_UNIT_ID = "ca-app-pub-3940256099942544/1033173712";

    private string _adUnitId;
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    /*
     * Google Mobile AdMob�� �ʱ�ȭ �մϴ�.
     */
    public void Init()
    {
        // ����� ���� SDK �ʱ�ȭ
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            Debug.Log("MobileAds.Initialize");

            LoadRewardedAd();
        });
    }

    /*
     * ���ο� ������ ���� �ε��մϴ�.
     */
    public void LoadRewardedAd()
    {

#if UNITY_EDITOR
        _adUnitId = REWARD_ADS_TEST_UNIT_ID;
        Debug.Log("Editor");
#elif UNITY_ANDROID
        _adUnitId = REWARD_ADS_UNIT_ID;
        Debug.Log("Android");
#else
        _adUnitId = "unexpected_platform";
#endif

        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        AdRequest adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log($"_adUnitId : {_adUnitId}");
        Debug.Log(adRequest.ToString());

        RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            _rewardedAd = ad;

            // _rewardedAd�� null�� �ƴ� ���� �̺�Ʈ �ڵ鷯 �߰�
            if (_rewardedAd != null)
            {
                _rewardedAd.OnAdFullScreenContentClosed += OnContentClosed;
                _rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
                {
                    Debug.LogError("Rewarded ad failed to open full screen content with error : " + adError);

                    // ���� ��ε�
                    LoadRewardedAd();
                };
            }

            //_rewardedAd.OnAdLoaded += HandleRewardedAdLoaded; // ���� �ε尡 �Ϸ�Ǹ� ȣ��
            //_rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad; // ���� �ε尡 �������� �� ȣ��
            //_rewardedAd.OnAdOpening += HandleRewardedAdOpening; // ���� ǥ�õ� �� ȣ��(��� ȭ���� ����)
            //_rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow; // ���� ǥ�ð� �������� �� ȣ��
            //_rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;// ���� ��û�� �� ������ �޾ƾ��� �� ȣ��
            //_rewardedAd.OnAdFullScreenContentClosed += OnContentClosed; // �ݱ� ��ư�� �����ų� �ڷΰ��� ��ư�� ���� ������ ���� ���� �� ȣ��

        });
    }

    /*
     * ���� �����մϴ�.
     */
    public void ShowRewardedAd(Action onAdCompleted)
    {
        const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
                if(onAdCompleted != null)
                {
                    onAdCompleted();
                }
            });
        }
        else
        {
            LoadRewardedAd(); // ���� �� �ε�
        }
    }

    private void OnContentClosed()
    {
        LoadRewardedAd();
    }
}
