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
               // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 전달받은 아이템 데이터
    /// 반환 결과 / 데이터 개수 / 데이터 정보(개수만큼) {아이템 코드, 아이템 이름, 아이템 능력}
    /// </summary>
    public bool ReceiveNecklaceData(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }


    /// <summary>
    /// 사용자가 보유한 아이템 데이터를 요청합니다.
    /// 패킷 크기 / 명령 / AccessToken크기 / Token
    /// </summary>
    public static ArraySegment<byte> GetUserNecklaceDataRequest(string token)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 사용자가 보유한 아이템 데이터
    /// 반환 결과 / 데이터 개수 / 데이터 정보 {아이템 코드}
    /// </summary>
    public bool ReceiveUserNecklaceData(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

}
