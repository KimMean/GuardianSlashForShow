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
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 클리어한 스테이지를 전달받습니다.
    /// 반환 결과 / 클리어한 스테이지
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool ReceiveUserClearStageResponse(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
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
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 스테이지 결과를 전달받습니다.
    /// 반환 결과 / 게임 결과 / 스테이지 / 보유한 코인 / 보유한 다이아
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool ReceiveStageResultResponse(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }
}
