using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class StagePacket
{
    /// <summary>
    /// 클리어한 최대 스테이지를 요청합니다.
    /// 패킷 크기 / 명령 / 토큰 사이즈 / 토큰
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 클리어한 스테이지를 전달받습니다.
    /// 반환 결과 / 클리어한 스테이지
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 게임 결과를 전송합니다.
    /// 패킷 크기 / 명령 / 토큰 사이즈 / 토큰 / 게임 결과 / 스테이지 / 획득한 코인 / 획득한 다이아
    /// </summary>
    /// <param name="token">토큰</param>
    /// <param name="state">게임 결과</param>
    /// <param name="stage">도전한 스테이지</param>
    /// <param name="coin">획득한 코인</param>
    /// <param name="diamond">획득한 다이아</param>
    /// <returns></returns>
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

    /// <summary>
    /// 스테이지 결과를 전달받습니다.
    /// 반환 결과 / 게임 결과 / 스테이지 / 보유한 코인 / 보유한 다이아
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
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
