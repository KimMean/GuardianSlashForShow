using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;
using static Packet;

public class PurchasePacket
{
    /*
     * 게임 내 재화로 구매합니다.
     * Command / Payment / Token / ProductID { / transactionID / receipt }
     */
    /// <summary>
    /// 아이템을 구매합니다.
    /// 패킷 크기 / 명령 / 지불 방식 / 토큰 / 제품 ID / {+ 식별코드 / 영수증 }
    /// </summary>
    /// <param name="payment"></param>
    /// <param name="token"></param>
    /// <param name="productID"></param>
    /// <param name="transactionID"></param>
    /// <param name="receipt"></param>
    /// <returns></returns>
    public static ArraySegment<byte> GetPurchaseRequest(Payment payment, string token, string productID, string transactionID = null, string receipt = null)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 구매 결과를 받습니다.
    /// 반환 결과 / 제품 종류
    /// 재화 구매시 + 코인 , 다이아
    /// 무기, 목걸이, 반지 구매시 + 아이템 코드 리스트와 고유 정보
    /// </summary>
    public bool ReceivePurchaseResponse(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
}
