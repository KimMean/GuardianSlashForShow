using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Packet;

public class CurrencyPacket
{

    public static ArraySegment<byte> GetCurrencyRequest(string token)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = 2;

        BitConverter.GetBytes((ushort)Command.Currency).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
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

    public bool CheckCurrencyResponse(ArraySegment<byte> buffer)
    {
        Debug.Log("Currency 받는 중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        int coin = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(uint);
        int dia = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(uint);

        GameManager.Instance.SetCoin(coin);
        GameManager.Instance.SetDiamond(dia);

        return true;
    }
}
