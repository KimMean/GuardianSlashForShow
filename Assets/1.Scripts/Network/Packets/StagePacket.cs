using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class StagePacket
{
    public static ArraySegment<byte> GetUserClearStageRequest(string token)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = 2;

        BitConverter.GetBytes((ushort)Command.ClearStage).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
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

    public bool ReceiveUserClearStageResponse(ArraySegment<byte> buffer)
    {
        Debug.Log("ReceiveUserClearStageResponse 받는 중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        uint stage = BitConverter.ToUInt32(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(uint);

        GameManager.Instance.SetClearStage((int)stage);

        return true;
    }

    public static ArraySegment<byte> GetStageResultRequest(string token, GameState state, int stage, int coin, int diamond)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = 2;

        BitConverter.GetBytes((ushort)Command.EndGame).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
        count += sizeof(ushort);

        byte[] userToken = Encoding.UTF8.GetBytes(token);
        ushort tokenSize = (ushort)userToken.Length;

        BitConverter.GetBytes(tokenSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        userToken.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += tokenSize;

        BitConverter.GetBytes((ushort)state).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        BitConverter.GetBytes((ushort)stage).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        BitConverter.GetBytes((ushort)Products.Coin).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        BitConverter.GetBytes(coin).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(int);

        BitConverter.GetBytes((ushort)Products.Diamond).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        BitConverter.GetBytes(diamond).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(int);

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);

        return SendBufferHelper.Close(count);
    }

    public bool ReceiveStageResultResponse(ArraySegment<byte> buffer)
    {
        Debug.Log("ReceiveUserClearStageResponse 받는 중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        bool gameResult = BitConverter.ToBoolean(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(bool);
        ushort stage = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        int coin = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(int);
        int dia = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(int);

        if(gameResult)
        {
            GameManager.Instance.StageClear((int)stage);
        }
        GameManager.Instance.SetCoin(coin);
        GameManager.Instance.SetDiamond(dia);
        
        return true;
    }
}
