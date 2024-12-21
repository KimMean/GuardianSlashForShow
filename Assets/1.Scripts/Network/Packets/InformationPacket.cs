using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Packet;

public class InformationPacket
{
    /// <summary>
    /// 앱 정보를 요청합니다.
    /// 패킷 크기 / 명령 / 사이즈 / 플랫폼
    /// </summary>
    /// <param name="platform">안드로이드</param>
    public static ArraySegment<byte> GetInformationData(string platform)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }
    /// <summary>
    /// 앱 정보를 받습니다.
    /// 반환 결과 / 버전 크기 / 버전 / 스토어 url 크기 / url
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool ReceiveInformationData(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }
}
