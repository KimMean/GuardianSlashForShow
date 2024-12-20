using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using static Packet;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            }
            return instance;
        }
    }

    SocketManager _SocketManager;
    //IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
    IPHostEntry ipHost = Dns.GetHostEntry("private");
    const int port = private;

    bool isInternetConnected;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        _SocketManager = new SocketManager();
        SocketInit();


    }

    private void Update()
    {

    }

    private void OnDestroy()
    {
        SocketDisconnect();
    }

    private void SocketInit()
    {
        IPAddress ipAddress = ipHost.AddressList[0];
        _SocketManager.Init(new IPEndPoint(ipAddress, port));

        StartCoroutine(CheckAppVersion());
    }

    /// <summary>
    /// 소켓이 연결되는 것을 기다린 후 서버의 앱 버전을 요청합니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckAppVersion()
    {
        while (true)
        {
            yield return null;
            Debug.Log(_SocketManager.GetIsConnected());

            if(_SocketManager.IsSocketConnected())
            {
                yield return new WaitForSeconds(0.1f);

                GetAppInformation();
                break;
            }
        }
    }

    /// <summary>
    /// 소켓 연결을 종료합니다.
    /// </summary>
    public void SocketDisconnect()
    {
        if(_SocketManager != null ) 
            _SocketManager.Disconnect();
    }

    /// <summary>
    /// 소켓이 연결되어있는지 확인합니다.
    /// </summary>
    /// <returns></returns>
    public bool GetIsConnected()
    {
        if (_SocketManager.GetIsConnected())
        {
            return true;
        }
        else
        {
            MessageManager.Instance.ShowMessage("연결이 원활하지 않습니다. 잠시 후 다시 시도해 주세요.");
            SocketInit();
            return false;
        }
    }
    /// <summary>
    /// TCP 소켓 연결을 주기적으로 확인합니다.
    /// </summary>
    public void Heartbeat()
    {
        if (!GetIsConnected()) return;

        _SocketManager.Send(HeartbeatPacket.GetHeartbeatPacket());
    }

    /// <summary>
    /// 앱의 최신 버전을 요청합니다.
    /// </summary>
    public void GetAppInformation()
    {
        if (!GetIsConnected()) return;

        _SocketManager.Send(InformationPacket.GetInformationData(GameManager.Instance.GetRuntimePlatform()));
    }

    /// <summary>
    /// 게스트 등록을 요청합니다.
    /// </summary>
    private void GuestRegistration()
    {
        if (!GetIsConnected()) return;

        _SocketManager.Send(LoginPacket.GetRegistrationRequest(Command.GuestSignUP));
    }

    /// <summary>
    /// 구글 아이디 등록을 요청합니다.
    /// 사용되지 않습니다.
    /// </summary>
    private void GoogleRegistration()
    {
        if (!GetIsConnected()) return;

        PlayGamesPlatform.Instance.Authenticate((SignInStatus status) =>
            {
                if(status == SignInStatus.Success)
                {
                    string userID = PlayGamesPlatform.Instance.GetUserId();
                    DataManager.Instance.SetUserGoogleID(userID);

                    Debug.Log($"Google Login Success, User ID : {userID}");

                    _SocketManager.Send(LoginPacket.GetRegistrationRequest(Command.GoogleSignUP, userID));
                }
                else
                {
                    Debug.Log("Google Play Games Sign in Failed");
                }
            }
        );
    }

    /// <summary>
    /// 로그인을 요청합니다.
    /// </summary>
    public void GuestLogin()
    {
        if (!GetIsConnected()) return;

        string uuid = DataManager.Instance.GetUserUUID();
        Debug.Log($"UUID : {uuid}");
        if (uuid == null)
        {
            GuestRegistration();
            return;
        }

        _SocketManager.Send(LoginPacket.LoginRequest(Command.GuestLogin, uuid));

        GameManager.Instance.SetAccessTime();
    }

    /// <summary>
    /// 구글 로그인을 요청합니다.
    /// </summary>
    public void GoogleLogin()
    {
        if (!GetIsConnected()) return;

        string userID = DataManager.Instance.GetUserGoogleID();
        Debug.Log($"Google ID : {userID}");
        if (userID == null)
        {
            GoogleRegistration();
            return;
        }

        _SocketManager.Send(LoginPacket.LoginRequest(Command.GoogleLogin, userID));
    }

    /// <summary>
    /// 클리어한 스테이지를 요청합니다.
    /// </summary>
    /// <returns></returns>
    public Task<bool> GetUserClearStage()
    {
        Debug.Log("GetUserClearStage");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(StagePacket.GetUserClearStageRequest(token));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.ClearStage] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.ClearStage] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 재화 정보를 요청합니다.
    /// </summary>
    /// <returns></returns>
    public Task<bool> GetUserCurrency()
    {
        Debug.Log("GetUserCurrency");
        string token = DataManager.Instance.GetAccessToken();

        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        _SocketManager.Send(CurrencyPacket.GetCurrencyRequest(token));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.Currency] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Currency] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 전체 무기 데이터를 요청합니다.
    /// </summary>
    /// <returns></returns>
    public Task<bool> GetWeaponData()
    {
        Debug.Log("GetWeaponData");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        _SocketManager.Send(WeaponPacket.GetWeaponDataRequest());

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.Weapon] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Weapon] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 사용자가 보유한 무기 데이터를 요청합니다.
    /// </summary>
    /// <returns></returns>
    public Task<bool> GetUserWeaponData()
    {
        Debug.Log("GetUserWeaponData");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(WeaponPacket.GetUserWeaponDataRequest(token));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.UserWeapon] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.UserWeapon] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 무기 강화를 요청합니다.
    /// </summary>
    /// <param name="itemCode">무기 코드</param>
    /// <param name="quantityRequire">강화 수량</param>
    /// <param name="cost">강화 비용</param>
    public void WeaponEnhancement(string itemCode, int quantityRequire, int cost)
    {
        if (!GetIsConnected()) return;

        //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(WeaponPacket.GetUserWeaponEnhancementRequest(token, itemCode, quantityRequire, cost));

    }

    /// <summary>
    /// 전체 목걸이 데이터를 요청합니다.
    /// </summary>
    public Task<bool> GetNecklaceData()
    {
        Debug.Log("GetNecklaceData");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        _SocketManager.Send(NecklacePacket.GetNecklaceDataRequest());

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.Necklace] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Necklace] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 사용자가 보유한 목걸이 데이터를 요청합니다.
    /// </summary>
    public Task<bool> GetUserNecklaceData()
    {
        Debug.Log("GetUserNecklaceData");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(NecklacePacket.GetUserNecklaceDataRequest(token));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.UserNecklace] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.UserNecklace] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 전체 반지 데이터를 요청합니다.
    /// </summary>
    public Task<bool> GetRingData()
    {
        Debug.Log("GetRingData");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        _SocketManager.Send(RingPacket.GetRingDataRequest());

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.Ring] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Ring] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 사용자가 보유한 반지 데이터를 요청합니다.
    /// </summary>
    public Task<bool> GetUserRingData()
    {
        Debug.Log("GetUserRingData");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(RingPacket.GetUserRingDataRequest(token));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.UserRing] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.UserRing] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 사용자가 마지막으로 장착했던 아이템 정보를 요청합니다.
    /// </summary>
    public Task<bool> GetUserEquipmentData()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(EquipmentPacket.GetUserEquipmentDataRequest(token));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.Equipment] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Equipment] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 장착한 아이템 변경을 요청합니다.
    /// </summary>
    /// <param name="item">무기, 목걸이, 반지</param>
    /// <param name="itemCode">아이템 코드</param>
    public void ChangeEquipment(Products item, string itemCode)
    {
        if (!GetIsConnected()) return;

        //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(EquipmentPacket.ChangeEquipmentRequest(token, item, itemCode));
    }

    /// <summary>
    /// 전체 상품 데이터를 요청합니다.
    /// </summary>
    public Task<bool> GetProductData()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        _SocketManager.Send(ProductPacket.GetProductDataRequest());

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.Product] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Product] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// 스테이지 결과 정보를 저장합니다.
    /// </summary>
    /// <param name="state">게임 결과</param>
    /// <param name="stage">도전한 스테이지</param>
    /// <param name="coin">스테이지에서 획득한 코인</param>
    /// <param name="diamond">스테이지에서 획득한 다이아몬드</param>
    public void GameEnd(GameState state, int stage, int coin, int diamond)
    {
        if (!GetIsConnected()) return;

        //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(StagePacket.GetStageResultRequest(token, state, stage, coin, diamond));
    }

    /// <summary>
    /// 아이템 구매를 요청합니다.
    /// </summary>
    /// <param name="payment">지불 방식</param>
    /// <param name="purchaseCode">구매 코드</param>
    /// <param name="transactionID">구매 식별자</param>
    /// <param name="receipt">영수증</param>
    public void ItemPurchase(Payment payment, string purchaseCode, string transactionID = null, string receipt = null)
    {
        if (!GetIsConnected()) return;

        //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(PurchasePacket.GetPurchaseRequest(payment, token, purchaseCode, transactionID, receipt));
    }

}
