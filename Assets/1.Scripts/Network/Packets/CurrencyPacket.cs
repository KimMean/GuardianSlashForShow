using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Packet;

public class CurrencyPacket
{

    /// <summary>
    /// 사용자의 재화 정보를 요청합니다.
    /// 패킷 크기 / 명령 / 토큰 사이즈 / 토큰
    /// </summary>
    /// <param name="token">토큰</param>
    public static ArraySegment<byte> GetCurrencyRequest(string token)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 사용자의 재화 정보를 받습니다.
    /// 반환 결과 / 코인 / 다이아
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool CheckCurrencyResponse(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }
}
