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
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
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
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
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
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 로그인을 요청합니다.
    /// 패킷 크기 / 명령 / 아이디 사이즈 / 아이디
    /// </summary>
    public static ArraySegment<byte> LoginRequest(Command command, string userId)
    {

                // 무작위 패킷 공격 방지를 위해 삭제합니다.

    }
    /// <summary>
    /// 로그인 결과를 받습니다.
    /// 반환 결과 / 데이터 크기 / 액세스 토큰
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool CheckLoginResponse(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }
}
