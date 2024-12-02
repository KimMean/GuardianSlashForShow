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
    IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());   // IP 주소 비공개
    const int _Port = 1234; // Port 번호 비공개


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        _SocketManager = new SocketManager();
        IPAddress _IPAddress = ipHost.AddressList[0];
        //Debug.Log(_IPAddress.ToString());
        _SocketManager.Init(new IPEndPoint(_IPAddress, _Port));

    }
    private void OnDestroy()
    {
        SocketDisconnect();
    }

    public void SocketDisconnect()
    {
        if(_SocketManager != null ) 
            _SocketManager.Disconnect();
    }
    /*
     * 게스트 등록을 요청합니다.
     */
    private void GuestRegistration()
    {
        _SocketManager.Send(LoginPacket.GetRegistrationRequest(Command.GuestSignUP));
    }
    private void GoogleRegistration()
    {
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

    /*
     * UUID가 있으면 로그인을 진행합니다.
     */
    public void GuestLogin()
    {
        string uuid = DataManager.Instance.GetUserUUID();
        Debug.Log($"UUID : {uuid}");
        if (uuid == null)
        {
            GuestRegistration();
            return;
        }

        _SocketManager.Send(LoginPacket.LoginRequest(Command.GuestLogin, uuid));
    }

    public void GoogleLogin()
    {
        string userID = DataManager.Instance.GetUserGoogleID();
        Debug.Log($"Google ID : {userID}");
        if (userID == null)
        {
            GoogleRegistration();
            return;
        }

        _SocketManager.Send(LoginPacket.LoginRequest(Command.GoogleLogin, userID));
    }


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

    public void WeaponEnhancement(string itemCode, int quantityRequire, int cost)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(WeaponPacket.GetUserWeaponEnhancementRequest(token, itemCode, quantityRequire, cost));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.WeaponEnhancement] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.WeaponEnhancement] += handler;
    }

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
    public Task<bool> ChangeEquipment(Products item, string itemCode)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(EquipmentPacket.ChangeEquipmentRequest(token, item, itemCode));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.ChangeEquipment] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.ChangeEquipment] += handler;

        return tcs.Task;
    }


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

    public void GameEnd(GameState state, int stage, int coin, int diamond)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(StagePacket.GetStageResultRequest(token, state, stage, coin, diamond));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.EndGame] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.EndGame] += handler;
    }

    public void ItemPurchase(Payment payment, string purchaseCode, string transactionID = null, string receipt = null)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(PurchasePacket.GetPurchaseRequest(payment, token, purchaseCode, transactionID, receipt));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // 이벤트 핸들러 제거
            PacketManager.Instance.RequestEvent[Command.Purchase] -= handler;

            // TaskCompletionSource에 결과 설정
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Purchase] += handler;
    }

}
