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
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort) * 2;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);
        BitConverter.GetBytes((ushort)Command.Product).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));

        return SendBufferHelper.Close(count);
    }

    /// <summary>
    /// 상품 데이터를 받습니다.
    /// 반환 결과 / 데이터 개수 / 데이터 정보 {제품 ID, 제품 이름, 필요한 재화, 가격}
    /// </summary>
    public bool ReceiveProductData(ArraySegment<byte> buffer)
    {
        Debug.Log("ReceiveProductData 받는 중");
        int packetHeaderSize = 2;
        // 헤더 유무
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;

        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        ushort dataCount = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        for (ushort i = 0; i < dataCount; i++)
        {
            // Product ID
            ushort idSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string productID = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, idSize);
            parsingCount += idSize;

            // Product Name
            ushort nameSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string productName = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, nameSize);
            parsingCount += nameSize;

            // Product CurrencyType
            Products currencyType = (Products)BitConverter.ToInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);

            // Product Price
            int price = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);

            //Debug.Log($"Product Packet Product ID : {productID}, ProductName : {productName}, CurrencyType : {currencyType}, Price : {price}");
            InAPP_ProductManager.Instance.SetInAPP_ProductData(productID, productName, currencyType, price);
        }

        return true;
    }
}
