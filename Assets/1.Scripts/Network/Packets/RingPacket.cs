using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class RingPacket
{
    public static ArraySegment<byte> GetRingDataRequest()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort) * 2;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);
        BitConverter.GetBytes((ushort)Command.Ring).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));

        return SendBufferHelper.Close(count);
    }

    public bool ReceiveRingData(ArraySegment<byte> buffer)
    {
        //Debug.Log("ReceiveRingData 받는 중");
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
            // Ring Code
            ushort codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string ringCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
            parsingCount += codeSize;


            // Ring Name
            ushort nameSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string ringName = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, nameSize);
            parsingCount += nameSize;

            // Ablility
            ushort attack = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            ushort gold = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            ushort jump = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);

            if (ringCode == "R000") continue;

            RingManager.Instance.SetRingData(ringCode, ringName, attack, gold, jump);
        }

        return true;
    }


    public static ArraySegment<byte> GetUserRingDataRequest(string token)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort);

        BitConverter.GetBytes((ushort)Command.UserRing).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
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

    public bool ReceiveUserRingData(ArraySegment<byte> buffer)
    {
        //Debug.Log("ReceiveUserRingData 받는 중");
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
            // Ring Code
            ushort codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string ringCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
            parsingCount += codeSize;


            RingManager.Instance.Rings[ringCode].SetIsPossess(true);
            //Debug.Log($"Ring Data Code : {ringCode}, IsHave.");
        }

        return true;
    }

}
