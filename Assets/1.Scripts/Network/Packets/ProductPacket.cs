using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class ProductPacket
{
    /// <summary>
    /// ��ǰ �����͸� ��û�մϴ�.
    /// </summary>
    public static ArraySegment<byte> GetProductDataRequest()
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// ��ǰ �����͸� �޽��ϴ�.
    /// ��ȯ ��� / ������ ���� / ������ ���� {��ǰ ID, ��ǰ �̸�, �ʿ��� ��ȭ, ����}
    /// </summary>
    public bool ReceiveProductData(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }
}
