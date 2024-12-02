using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Packet;

public class PacketManager
{
    private static PacketManager instance;
    public static PacketManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PacketManager();
            }
            return instance;
        }
    }

    public Dictionary<Command, Action<bool>> RequestEvent;

    PacketManager()
    {
        RequestEvent = new Dictionary<Command, Action<bool>>();
        foreach (Command command in Enum.GetValues(typeof(Command)))
        {
            RequestEvent[command] = null;
        }
    }

    public void OnReceivePacket(ArraySegment<byte> buffer)
    {
        ushort cmd = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        int commandSize = sizeof(ushort);

        Command command = (Command)cmd;
        Debug.Log($"Command : {command}");

        switch (command)
        {
            case Command.NONE: break;
            case Command.GuestSignUP:
                {
                    LoginPacket loginPacket = new LoginPacket();
                    bool result = loginPacket.CheckRegistrationResponse(command, new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));
                    
                    if (result) NetworkManager.Instance.GuestLogin();
                    break;
                }
            case Command.GoogleSignUP:
                {
                    LoginPacket loginPacket = new LoginPacket();
                    bool result = loginPacket.CheckRegistrationResponse(command, new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));
                    
                    if (result) NetworkManager.Instance.GoogleLogin();
                    break;
                }
            case Command.GuestLogin:
                {
                    LoginPacket loginPacket = new LoginPacket();
                    bool result = loginPacket.CheckLoginResponse(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    MainThreadDispatcher.Instance.RunOnMainThread(() =>
                    {
                        if (result)
                        {
                            LoadingManager.LoadScene("Lobby", true);
                        }
                    });
                    break;
                }
            case Command.GoogleLogin:
                {
                    LoginPacket loginPacket = new LoginPacket();
                    bool result = loginPacket.CheckLoginResponse(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    MainThreadDispatcher.Instance.RunOnMainThread(() =>
                    {
                        if (result)
                        {
                            LoadingManager.LoadScene("Lobby", true);
                        }
                    });
                    break;
                }
            case Command.ClearStage:
                {
                    StagePacket stagePacket = new StagePacket();
                    bool result = stagePacket.ReceiveUserClearStageResponse(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.Currency:
                {
                    CurrencyPacket currencyPacket = new CurrencyPacket();
                    bool result = currencyPacket.CheckCurrencyResponse(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.Weapon:
                {
                    WeaponPacket weaponPacket = new WeaponPacket();
                    bool result = weaponPacket.ReceiveWeaponData(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.UserWeapon:
                {
                    WeaponPacket weaponPacket = new WeaponPacket();
                    bool result = weaponPacket.ReceiveUserWeaponData(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.WeaponEnhancement:
                {
                    WeaponPacket weaponPacket = new WeaponPacket();
                    bool result = weaponPacket.ReceiveUserWeaponEnhancementData(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.Necklace:
                {
                    NecklacePacket necklacePacket = new NecklacePacket();
                    bool result = necklacePacket.ReceiveNecklaceData(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.UserNecklace:
                {
                    NecklacePacket necklacePacket = new NecklacePacket();
                    bool result = necklacePacket.ReceiveUserNecklaceData(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.Ring:
                {
                    RingPacket ringPacket = new RingPacket();
                    bool result = ringPacket.ReceiveRingData(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.UserRing:
                {
                    RingPacket ringPacket = new RingPacket();
                    bool result = ringPacket.ReceiveUserRingData(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.Equipment:
                {
                    EquipmentPacket equipmentPacket = new EquipmentPacket();
                    bool result = equipmentPacket.ReceiveUserEquipmentDataResponse(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.ChangeEquipment:
                {
                    Debug.Log("ChangeEquipment");
                    EquipmentPacket equipmentPacket = new EquipmentPacket();
                    bool result = equipmentPacket.ReceiveChangeEquipmentDataResponse(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.EndGame:
                {
                    StagePacket stagePacket = new StagePacket();
                    bool result = stagePacket.ReceiveStageResultResponse(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.Purchase:
                {
                    PurchasePacket purchasePacket = new PurchasePacket();
                    bool result = purchasePacket.ReceivePurchaseResponse(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            case Command.Product:
                {
                    ProductPacket productPacket = new ProductPacket();
                    bool result = productPacket.ReceiveProductData(new ArraySegment<byte>(buffer.Array, buffer.Offset + commandSize, buffer.Count - commandSize));

                    RequestEvent[command].Invoke(result);
                    break;
                }
            default:
                {
                    Debug.Log($"Packet Manager Error : {command}");
                    break;
                }
        }
    }
}
