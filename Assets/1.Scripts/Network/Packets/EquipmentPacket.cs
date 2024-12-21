using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class EquipmentPacket
{
    /// <summary>
    /// 장착한 아이템 정보를 요청합니다.
    /// 패킷 크기 / 명령 / 토큰 사이즈 / 토큰
    /// </summary>
    /// <param name="token">토큰</param>
    public static ArraySegment<byte> GetUserEquipmentDataRequest(string token)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }
    /// <summary>
    /// 장착한 아이템 정보를 받습니다.
    /// 반환 결과 / 무기 코드 사이즈 / 무기 코드 / 목걸이 코드 사이즈 / 목걸이 코드 / 반지 코드 사이즈 / 반지 코드
    /// </summary>
    public bool ReceiveUserEquipmentDataResponse(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 장착한 아이템의 변경을 요청합니다.
    /// 패킷 크기 / 명령 / 토큰 사이즈 / 토큰 / 아이템 종류 / 아이템 코드 사이즈 / 아이템 코드
    /// </summary>
    /// <param name="token">토큰</param>
    /// <param name="product">무기, 목걸이, 반지</param>
    /// <param name="itemCode">아이템 코드</param>
    /// <returns></returns>
    public static ArraySegment<byte> ChangeEquipmentRequest(string token, Products product, string itemCode)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 변경한 아이템 정보를 받습니다.
    /// 반환 결과 / 아이템 종류 / 코드 사이즈 / 아이템 코드
    /// </summary>
    public bool ReceiveChangeEquipmentDataResponse(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
        
    }
}
