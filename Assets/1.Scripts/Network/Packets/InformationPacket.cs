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
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort);

        BitConverter.GetBytes((ushort)Command.Information).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
        count += sizeof(ushort);

        // 토큰
        byte[] platformData = Encoding.UTF8.GetBytes(platform);
        ushort platformSize = (ushort)platformData.Length;

        BitConverter.GetBytes(platformSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        platformData.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += platformSize;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);

        return SendBufferHelper.Close(count);
    }
    /// <summary>
    /// 앱 정보를 받습니다.
    /// 반환 결과 / 버전 크기 / 버전 / 스토어 url 크기 / url
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool ReceiveInformationData(ArraySegment<byte> buffer)
    {
        Debug.Log("ReceiveInformationData 받는 중");
        
        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;


        // App Version
        ushort versionSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);
        string version = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, versionSize);
        parsingCount += versionSize;

        // URL
        ushort urlSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);
        string url = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, urlSize);
        parsingCount += urlSize;

        GameManager.Instance.SetAppInformation(version, url);

        return true;
    }
}
