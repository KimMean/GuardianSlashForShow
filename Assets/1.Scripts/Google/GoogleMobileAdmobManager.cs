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

    /// <summary>
    /// 리워드 광고 테스트 ID
    /// </summary>
    const string REWARD_ADS_TEST_UNIT_ID =  "ca-app-pub-3940256099942544/5224354917";
    /// <summary>
    /// 리워드 광고 ID
    /// </summary>
    const string REWARD_ADS_UNIT_ID = "ca-app-pub-6690936278194276/3952341079";
    
    // 전면 광고 테스트 ID (사용되지 않습니다.)
    const string Interstitial_ADS_TEST_UNIT_ID = "ca-app-pub-3940256099942544/1033173712";

    /// <summary>
    /// 모바일 광고 SDK가 초기화 되면서 광고 ID가 설정됩니다.
    /// </summary>
    private string _adUnitId;
    /// <summary>
    /// 리워드 광고가 초기화 된 후에 정보를 가지고 있습니다.
    /// </summary>
    private RewardedAd _rewardedAd;
    // 전면 광고는 사용되지 않습니다.
    private InterstitialAd _interstitialAd;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    /// <summary>
    /// Google Mobile AdMob을 초기화 합니다.
    /// </summary>
    public void Init()
    {
        // 모바일 광고 SDK 초기화
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            Debug.Log("MobileAds.Initialize");

            LoadRewardedAd();
        });
    }

    /// <summary>
    /// 새로운 보상형 광고를 로드합니다.
    /// </summary>
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

            // _rewardedAd가 null이 아닐 때만 이벤트 핸들러 추가
            if (_rewardedAd != null)
            {
                _rewardedAd.OnAdFullScreenContentClosed += OnContentClosed;
                _rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
                {
                    Debug.LogError("Rewarded ad failed to open full screen content with error : " + adError);

                    // 광고 재로드
                    LoadRewardedAd();
                };
            }

            //_rewardedAd.OnAdLoaded += HandleRewardedAdLoaded; // 광고 로드가 완료되면 호출
            //_rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad; // 광고 로드가 실패했을 때 호출
            //_rewardedAd.OnAdOpening += HandleRewardedAdOpening; // 광고가 표시될 때 호출(기기 화면을 덮음)
            //_rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow; // 광고 표시가 실패했을 때 호출
            //_rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;// 광고를 시청한 후 보상을 받아야할 때 호출
            //_rewardedAd.OnAdFullScreenContentClosed += OnContentClosed; // 닫기 버튼을 누르거나 뒤로가기 버튼을 눌러 동영상 광고를 닫을 때 호출

        });
    }

    /// <summary>
    /// 보상형 광고를 보여줍니다.
    /// </summary>
    /// <param name="onAdCompleted">광고 시청 후 알림 받을 Action을 지정합니다.</param>
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
            LoadRewardedAd(); // 광고 재 로드
        }
    }

    /// <summary>
    /// 광고를 시청하고 사용자가 닫기를 누르면 호출됩니다.
    /// </summary>
    private void OnContentClosed()
    {
        LoadRewardedAd();
    }
}
