using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class NecklacePacket
{
    /// <summary>
    /// 아이템 데이터를 요청합니다.
    /// 패킷 사이즈 / 명령
    /// </summary>
    public static ArraySegment<byte> GetNecklaceDataRequest()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort) * 2;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);
        BitConverter.GetBytes((ushort)Command.Necklace).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));

        return SendBufferHelper.Close(count);
    }

    /// <summary>
    /// 전달받은 아이템 데이터
    /// 반환 결과 / 데이터 개수 / 데이터 정보(개수만큼) {아이템 코드, 아이템 이름, 아이템 능력}
    /// </summary>
    public bool ReceiveNecklaceData(ArraySegment<byte> buffer)
    {
        //Debug.Log("ReceiveNecklaceData 받는 중");
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
            // Necklace Code
            ushort codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string necklaceCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
            parsingCount += codeSize;

            // Necklace Name
            ushort nameSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string necklaceName = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, nameSize);
            parsingCount += nameSize;

            // Ablility
            ushort twilight = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            ushort varVoid = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            ushort hell = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);

            if (necklaceCode == "N000") continue;

            NecklaceManager.Instance.SetNecklaceData(necklaceCode, necklaceName, twilight, varVoid, hell);
        }

        return true;
    }


    /// <summary>
    /// 사용자가 보유한 아이템 데이터를 요청합니다.
    /// 패킷 크기 / 명령 / AccessToken크기 / Token
    /// </summary>
    public static ArraySegment<byte> GetUserNecklaceDataRequest(string token)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort);

        BitConverter.GetBytes((ushort)Command.UserNecklace).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
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
    /// 사용자가 보유한 아이템 데이터
    /// 반환 결과 / 데이터 개수 / 데이터 정보 {아이템 코드}
    /// </summary>
    public bool ReceiveUserNecklaceData(ArraySegment<byte> buffer)
    {
        //Debug.Log("ReceiveUserNecklaceData 받는 중");
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
            // Necklace Code
            ushort codeSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string necklaceCode = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, codeSize);
            parsingCount += codeSize;

            NecklaceManager.Instance.Necklaces[necklaceCode].SetIsPossess(true);
            //Debug.Log($"Necklace Data Code : {necklaceCode}, IsHave. ");
        }

        return true;
    }

}
