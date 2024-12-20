using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Packet;

public class LoginPacket
{
    const ushort UUID_SIZE = 36;
    const ushort ACCESS_TOKEN_SIZE = 12;

    /// <summary>
    /// 서버에 가입 요청
    /// 사용되지 않습니다.
    /// </summary>
    /// <returns></returns>
    public static ArraySegment<byte> GetRegistrationRequest()
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = 4;
        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);
        BitConverter.GetBytes((ushort)Command.GuestSignUP).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
    
    /*
     * 
     */
    /// <summary>
    /// 서버에 회원 등록 요청
    /// 데이터 크기 / 커맨드 / 유저 ID(email @gmail.com)
    /// 게스트의 경우 서버에서 UUID를 생성하여 반환합니다.
    /// </summary>
    public static ArraySegment<byte> GetRegistrationRequest(Command command, string userId = null)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort);
        BitConverter.GetBytes((ushort)command).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
        count += sizeof(ushort);

        if(command == Command.GoogleSignUP)
        {
            byte[] id = Encoding.UTF8.GetBytes(userId);
            ushort idSize = (ushort)id.Length;

            BitConverter.GetBytes(idSize).CopyTo(openSegment.Array, openSegment.Offset + count);
            count += sizeof(ushort);
            id.CopyTo(openSegment.Array, openSegment.Offset + count);
            count += idSize;
        }

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);
        return SendBufferHelper.Close(count);
    }

    /// <summary>
    /// 등록 결과를 받습니다.
    /// 반환 결과 / 데이터 크기 / UUID / 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool CheckRegistrationResponse(Command command, ArraySegment<byte> buffer)
    {
        Debug.Log("회원 등록중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);
        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        string userid = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, dataSize);
        Debug.Log($"회원 등록 완료 전 UUID : {userid}");

        if(command == Command.GuestSignUP)
            DataManager.Instance.SetUserUUID(userid);
        else if(command == Command.GoogleSignUP)
            DataManager.Instance.SetUserGoogleID(userid);

        Debug.Log($"회원 등록 완료 UUID : {userid}");

        return true;
    }

    /// <summary>
    /// 로그인을 요청합니다.
    /// 패킷 크기 / 명령 / 아이디 사이즈 / 아이디
    /// </summary>
    public static ArraySegment<byte> LoginRequest(Command command, string userId)
    {

        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort);

        // Command 입력
        BitConverter.GetBytes((ushort)command).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        byte[] userID = Encoding.UTF8.GetBytes(userId);
        ushort idSize = (ushort)userID.Length;

        BitConverter.GetBytes(idSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        userID.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += idSize;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);

        return SendBufferHelper.Close(count);

    }
    /// <summary>
    /// 로그인 결과를 받습니다.
    /// 반환 결과 / 데이터 크기 / 액세스 토큰
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool CheckLoginResponse(ArraySegment<byte> buffer)
    {
        //Debug.Log("게스트 로그인");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);
        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        if (dataSize != ACCESS_TOKEN_SIZE)
            return false;

        string token = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, dataSize);
        DataManager.Instance.SetAccessToken(token);
        //Debug.Log($"로그인 완료  : {token}");

        return true;
    }
}
