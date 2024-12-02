using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class WeaponPacket
{
    public static ArraySegment<byte> GetWeaponDataRequest()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort) * 2;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);
        BitConverter.GetBytes((ushort)Command.Weapon).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));

        return SendBufferHelper.Close(count);
    }

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

        //Debug.Log($"Weapon itemCode : {itemCode}, Level : {enhancementLevel}, Quantity : {quantity}, AtkLevel : {attackLevel}");
        //MainThreadDispatcher.Instance.Enqueue(() =>
        //{
            WeaponManager.Instance.SetUserWeaponData(itemCode, enhancementLevel, quantity, attackLevel);
            GameManager.Instance.SetCoin(coin);
        //});
        

        return true;
    }
}
