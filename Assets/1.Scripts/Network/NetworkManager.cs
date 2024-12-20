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
    /// ������ ����Ǵ� ���� ��ٸ� �� ������ �� ������ ��û�մϴ�.
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
    /// ���� ������ �����մϴ�.
    /// </summary>
    public void SocketDisconnect()
    {
        if(_SocketManager != null ) 
            _SocketManager.Disconnect();
    }

    /// <summary>
    /// ������ ����Ǿ��ִ��� Ȯ���մϴ�.
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
            MessageManager.Instance.ShowMessage("������ ��Ȱ���� �ʽ��ϴ�. ��� �� �ٽ� �õ��� �ּ���.");
            SocketInit();
            return false;
        }
    }
    /// <summary>
    /// TCP ���� ������ �ֱ������� Ȯ���մϴ�.
    /// </summary>
    public void Heartbeat()
    {
        if (!GetIsConnected()) return;

        _SocketManager.Send(HeartbeatPacket.GetHeartbeatPacket());
    }

    /// <summary>
    /// ���� �ֽ� ������ ��û�մϴ�.
    /// </summary>
    public void GetAppInformation()
    {
        if (!GetIsConnected()) return;

        _SocketManager.Send(InformationPacket.GetInformationData(GameManager.Instance.GetRuntimePlatform()));
    }

    /// <summary>
    /// �Խ�Ʈ ����� ��û�մϴ�.
    /// </summary>
    private void GuestRegistration()
    {
        if (!GetIsConnected()) return;

        _SocketManager.Send(LoginPacket.GetRegistrationRequest(Command.GuestSignUP));
    }

    /// <summary>
    /// ���� ���̵� ����� ��û�մϴ�.
    /// ������ �ʽ��ϴ�.
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
    /// �α����� ��û�մϴ�.
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
    /// ���� �α����� ��û�մϴ�.
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
    /// Ŭ������ ���������� ��û�մϴ�.
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
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.ClearStage] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.ClearStage] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// ��ȭ ������ ��û�մϴ�.
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
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.Currency] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Currency] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// ��ü ���� �����͸� ��û�մϴ�.
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
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.Weapon] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Weapon] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// ����ڰ� ������ ���� �����͸� ��û�մϴ�.
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
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.UserWeapon] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.UserWeapon] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// ���� ��ȭ�� ��û�մϴ�.
    /// </summary>
    /// <param name="itemCode">���� �ڵ�</param>
    /// <param name="quantityRequire">��ȭ ����</param>
    /// <param name="cost">��ȭ ���</param>
    public void WeaponEnhancement(string itemCode, int quantityRequire, int cost)
    {
        if (!GetIsConnected()) return;

        //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(WeaponPacket.GetUserWeaponEnhancementRequest(token, itemCode, quantityRequire, cost));

    }

    /// <summary>
    /// ��ü ����� �����͸� ��û�մϴ�.
    /// </summary>
    public Task<bool> GetNecklaceData()
    {
        Debug.Log("GetNecklaceData");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        _SocketManager.Send(NecklacePacket.GetNecklaceDataRequest());

        Action<bool> handler = null;
        handler = (result) =>
        {
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.Necklace] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Necklace] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// ����ڰ� ������ ����� �����͸� ��û�մϴ�.
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
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.UserNecklace] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.UserNecklace] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// ��ü ���� �����͸� ��û�մϴ�.
    /// </summary>
    public Task<bool> GetRingData()
    {
        Debug.Log("GetRingData");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        _SocketManager.Send(RingPacket.GetRingDataRequest());

        Action<bool> handler = null;
        handler = (result) =>
        {
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.Ring] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Ring] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// ����ڰ� ������ ���� �����͸� ��û�մϴ�.
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
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.UserRing] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.UserRing] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// ����ڰ� ���������� �����ߴ� ������ ������ ��û�մϴ�.
    /// </summary>
    public Task<bool> GetUserEquipmentData()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(EquipmentPacket.GetUserEquipmentDataRequest(token));

        Action<bool> handler = null;
        handler = (result) =>
        {
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.Equipment] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Equipment] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// ������ ������ ������ ��û�մϴ�.
    /// </summary>
    /// <param name="item">����, �����, ����</param>
    /// <param name="itemCode">������ �ڵ�</param>
    public void ChangeEquipment(Products item, string itemCode)
    {
        if (!GetIsConnected()) return;

        //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(EquipmentPacket.ChangeEquipmentRequest(token, item, itemCode));
    }

    /// <summary>
    /// ��ü ��ǰ �����͸� ��û�մϴ�.
    /// </summary>
    public Task<bool> GetProductData()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        _SocketManager.Send(ProductPacket.GetProductDataRequest());

        Action<bool> handler = null;
        handler = (result) =>
        {
            // �̺�Ʈ �ڵ鷯 ����
            PacketManager.Instance.RequestEvent[Command.Product] -= handler;

            // TaskCompletionSource�� ��� ����
            tcs.SetResult(result);
        };

        PacketManager.Instance.RequestEvent[Command.Product] += handler;

        return tcs.Task;
    }

    /// <summary>
    /// �������� ��� ������ �����մϴ�.
    /// </summary>
    /// <param name="state">���� ���</param>
    /// <param name="stage">������ ��������</param>
    /// <param name="coin">������������ ȹ���� ����</param>
    /// <param name="diamond">������������ ȹ���� ���̾Ƹ��</param>
    public void GameEnd(GameState state, int stage, int coin, int diamond)
    {
        if (!GetIsConnected()) return;

        //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(StagePacket.GetStageResultRequest(token, state, stage, coin, diamond));
    }

    /// <summary>
    /// ������ ���Ÿ� ��û�մϴ�.
    /// </summary>
    /// <param name="payment">���� ���</param>
    /// <param name="purchaseCode">���� �ڵ�</param>
    /// <param name="transactionID">���� �ĺ���</param>
    /// <param name="receipt">������</param>
    public void ItemPurchase(Payment payment, string purchaseCode, string transactionID = null, string receipt = null)
    {
        if (!GetIsConnected()) return;

        //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string token = DataManager.Instance.GetAccessToken();
        _SocketManager.Send(PurchasePacket.GetPurchaseRequest(payment, token, purchaseCode, transactionID, receipt));
    }

}
