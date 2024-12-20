using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class WeaponPacket
{
    /// <summary>
    /// 전체 무기 데이터 패킷
    /// 패킷 크기 / 패킷 명령
    /// </summary>
    public static ArraySegment<byte> GetWeaponDataRequest()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort) * 2;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);
        BitConverter.GetBytes((ushort)Command.Weapon).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));

        return SendBufferHelper.Close(count);
    }

    /// <summary>
    /// 전달받은 무기 데이터
    /// 패킷 명령 / 반환 결과 / 데이터 개수 / 데이터 정보(개수만큼) {무기 코드, 무기 이름, 무기 기본 공격 레벨}
    /// </summary>
    public bool ReceiveWeaponData(ArraySegment<byte> buffer)
    {
        Debug.Log("ReceiveWeaponData 받는 중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        ushort dataCount = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        for(ushort i = 0; i < dataCount; i++)
        {
            // Weapon Code
            ushort codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string weaponCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
            parsingCount += codeSize;

            // Weapon Name
            ushort nameSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string weaponName = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, nameSize);
            parsingCount += nameSize;
            // Weapon Attack Level
            ushort weaponAttackLevel = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);

            if (weaponCode == "W000") continue;
            WeaponManager.Instance.SetWeaponData(weaponCode, weaponName, weaponAttackLevel);
        }

        return true;
    }

    /// <summary>
    /// 사용자가 보유한 무기 데이터를 요청합니다.
    /// 패킷 크기 / 명령 / AccessToken크기 / Token
    /// </summary>
    public static ArraySegment<byte> GetUserWeaponDataRequest(string token)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort);

        BitConverter.GetBytes((ushort)Command.UserWeapon).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
        count += sizeof(ushort);

        // 토큰
        byte[] userToken = Encoding.UTF8.GetBytes(token);
        ushort tokenSize = (ushort)userToken.Length;

        BitConverter.GetBytes(tokenSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        userToken.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += tokenSize;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);

        return SendBufferHelper.Close(count);
    }

    /// <summary>
    /// 사용자가 보유한 무기 데이터
    /// 반환 결과 / 데이터 개수 / 데이터 정보 {무기 코드, 강화 레벨, 수량, 공격 레벨}
    /// </summary>
    public bool ReceiveUserWeaponData(ArraySegment<byte> buffer)
    {
        //Debug.Log("ReceiveUserWeaponData 받는 중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        ushort dataCount = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        for (ushort i = 0; i < dataCount; i++)
        {
            // Weapon Code
            ushort codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string weaponCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
            parsingCount += codeSize;

            // Weapon Enhancement Level
            ushort enhancementLevel = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            // Weapon Quantity
            ushort quantity = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            // Weapon Attack Level
            ushort attackLevel = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);

            //Debug.Log($"Weapon Data Code : {weaponCode}, Level : {enhancementLevel}, Quantity : {quantity}, AtkLevel : {attackLevel}");
            WeaponManager.Instance.SetUserWeaponData(weaponCode, enhancementLevel, quantity, attackLevel);
        }

        return true;
    }
    /// <summary>
    /// 무기 강화를 요청합니다.
    /// 패킷 크기 / 명령 / 토큰 사이즈 / 토큰 / 아이템 코드 사이즈 / 아이템 코드 / 필요한 무기 수량 / 강화 비용
    /// </summary>
    /// <param name="token">유저토큰</param>
    /// <param name="itemCode">아이템 코드</param>
    /// <param name="quantityRequire">강화 수량</param>
    /// <param name="cost">강화 비용</param>
    /// <returns></returns>
    public static ArraySegment<byte> GetUserWeaponEnhancementRequest(string token, string itemCode, int quantityRequire, int cost)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort);

        BitConverter.GetBytes((ushort)Command.WeaponEnhancement).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
        count += sizeof(ushort);

        // 토큰
        byte[] userToken = Encoding.UTF8.GetBytes(token);
        ushort tokenSize = (ushort)userToken.Length;

        BitConverter.GetBytes(tokenSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        userToken.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += tokenSize;

        // 아이템 코드
        byte[] code = Encoding.UTF8.GetBytes(itemCode);
        ushort codeSize = (ushort)code.Length;

        BitConverter.GetBytes(codeSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        code.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += codeSize;

        // upgrade Quantity Required
        BitConverter.GetBytes((ushort)quantityRequire).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        // upgrade Cost
        BitConverter.GetBytes(cost).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(int);

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);

        return SendBufferHelper.Close(count);
    }
    /// <summary>
    /// 무기 강화 결과를 전달받습니다.
    /// 반환 결과 / 코드 사이즈 / 코드 / 강화 레벨 / 공격 레벨 / 남은 수량 / 남은 재화
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool ReceiveUserWeaponEnhancementData(ArraySegment<byte> buffer)
    {
        Debug.Log("ReceiveUserWeaponData 받는 중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;

        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        // Weapon Code
        ushort codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);
        string itemCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
        parsingCount += codeSize;

        ushort enhancementLevel = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);
        ushort attackLevel = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);
        ushort quantity = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        int coin = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(int);

        WeaponManager.Instance.SetUserWeaponData(itemCode, enhancementLevel, quantity, attackLevel);
        GameManager.Instance.SetCoin(coin);
        

        return true;
    }
}
