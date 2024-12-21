using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class ProductPacket
{
    /// <summary>
    /// 상품 데이터를 요청합니다.
    /// </summary>
    public static ArraySegment<byte> GetProductDataRequest()
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }

    /// <summary>
    /// 상품 데이터를 받습니다.
    /// 반환 결과 / 데이터 개수 / 데이터 정보 {제품 ID, 제품 이름, 필요한 재화, 가격}
    /// </summary>
    public bool ReceiveProductData(ArraySegment<byte> buffer)
    {
                // 무작위 패킷 공격 방지를 위해 삭제합니다.
    }
}
