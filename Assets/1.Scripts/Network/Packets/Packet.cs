using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packet
{
    /// <summary>
    /// 명령
    /// </summary>
    public enum Command
    {
        NONE = 0,
        Heartbeat = 1,
        Information = 2,
        GuestSignUP = 10,
        GoogleSignUP = 11,
        GuestLogin = 20,
        GoogleLogin = 21,
        ClearStage = 100,
        Currency = 200,
        Weapon = 300,
        UserWeapon = 310,
        WeaponEnhancement = 320,
        Necklace = 400,
        UserNecklace = 410,
        Ring = 500,
        UserRing = 510,
        Equipment = 600,
        ChangeEquipment = 610,
        EndGame = 700,
        Purchase = 1000,
        Product = 1100,

        MAX = 65535
    }

    /// <summary>
    /// 반환 결과
    /// </summary>
    public enum ResultCommand
    {
        NONE = 0,
        Success = 1,
        Failed = 2,
        MAX = 65535
    }

    /// <summary>
    /// 제품
    /// </summary>
    public enum Products
    {
        NONE = 0,
        Coin = 10,
        Diamond = 20,
        Weapon = 30,
        Necklace = 40,
        Ring = 50,
        KRW = 100,

        MAX = 65535
    }

    /// <summary>
    /// 지불 방식
    /// </summary>
    public enum Payment
    {
        Local = 0,
        Google = 1,
    }

    /// <summary>
    /// 게임 결과 
    /// </summary>
    public enum GameState
    {
        GameOver = 0,
        GameClear = 1,
    }
}
