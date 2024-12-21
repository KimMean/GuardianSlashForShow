using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Packet;

public class HeartbeatPacket
{
    /// <summary>
    /// 서버와 지속적인 연결을 위해 하트비트 패킷을 전송합니다.
    /// 패킷 크기 / 명령
    /// </summary>
    /// <returns></returns>
    public static ArraySegment<byte> GetHeartbeatPacket()
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }
}
