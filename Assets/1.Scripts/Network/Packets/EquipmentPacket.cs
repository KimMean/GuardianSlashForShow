using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class EquipmentPacket
{
    /// <summary>
    /// 장착한 아이템 정보를 요청합니다.
    /// 패킷 크기 / 명령 / 토큰 사이즈 / 토큰
    /// </summary>
    /// <param name="token">토큰</param>
    public static ArraySegment<byte> GetUserEquipmentDataRequest(string token)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = 2;

        BitConverter.GetBytes((ushort)Command.Equipment).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
        count += sizeof(ushort);

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
    /// 장착한 아이템 정보를 받습니다.
    /// 반환 결과 / 무기 코드 사이즈 / 무기 코드 / 목걸이 코드 사이즈 / 목걸이 코드 / 반지 코드 사이즈 / 반지 코드
    /// </summary>
    public bool ReceiveUserEquipmentDataResponse(ArraySegment<byte> buffer)
    {
        Debug.Log("EquipmentData 받는 중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        // string code 3개
        // 무기
        ushort codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);
        string weaponCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
        parsingCount += codeSize;
        // 목걸이
        codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);
        string necklaceCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
        parsingCount += codeSize;
        // 반지
        codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);
        string ringCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
        parsingCount += codeSize;

        GameManager.Instance.SetEquipmentWeapon(weaponCode);
        GameManager.Instance.SetEquipmentNecklace(necklaceCode);
        GameManager.Instance.SetEquipmentRing(ringCode);

        //Debug.Log($"Equipments, Weapon : {weaponCode}, Necklace : {necklaceCode}, Ring : {ringCode}");

        return true;
    }

    /// <summary>
    /// 장착한 아이템의 변경을 요청합니다.
    /// 패킷 크기 / 명령 / 토큰 사이즈 / 토큰 / 아이템 종류 / 아이템 코드 사이즈 / 아이템 코드
    /// </summary>
    /// <param name="token">토큰</param>
    /// <param name="product">무기, 목걸이, 반지</param>
    /// <param name="itemCode">아이템 코드</param>
    /// <returns></returns>
    public static ArraySegment<byte> ChangeEquipmentRequest(string token, Products product, string itemCode)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = 2;

        BitConverter.GetBytes((ushort)Command.ChangeEquipment).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        // User Token
        byte[] userToken = Encoding.UTF8.GetBytes(token);
        ushort tokenSize = (ushort)userToken.Length;

        BitConverter.GetBytes(tokenSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        userToken.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += tokenSize;

        // Weapon, Necklace, Ring
        BitConverter.GetBytes((ushort)product).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        // Item Code 
        byte[] code = Encoding.UTF8.GetBytes(itemCode);
        ushort codeSize = (ushort)code.Length;

        BitConverter.GetBytes(codeSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        code.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += codeSize;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);

        return SendBufferHelper.Close(count);
    }

    /// <summary>
    /// 변경한 아이템 정보를 받습니다.
    /// 반환 결과 / 아이템 종류 / 코드 사이즈 / 아이템 코드
    /// </summary>
    public bool ReceiveChangeEquipmentDataResponse(ArraySegment<byte> buffer)
    {
        Debug.Log("EquipmentData 받는 중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        Products product = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        // string code 
        ushort codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);
        string itemCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
        parsingCount += codeSize;

        Debug.Log($"Code : {itemCode}");

        switch (product)
        {
            case Products.Weapon:
                GameManager.Instance.SetEquipmentWeapon(itemCode);
                break;
            case Products.Necklace:
                GameManager.Instance.SetEquipmentNecklace(itemCode);
                break;
            case Products.Ring:
                GameManager.Instance.SetEquipmentRing(itemCode);
                break;
            default:
                Debug.Log("Error");
                break;
        }
        

        return true;
    }
}
