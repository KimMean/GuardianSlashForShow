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
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        ushort count = sizeof(ushort);

        BitConverter.GetBytes((ushort)Command.Heartbeat).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        // 총 사이즈 입력
        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);

        return SendBufferHelper.Close(count);
    }
}
