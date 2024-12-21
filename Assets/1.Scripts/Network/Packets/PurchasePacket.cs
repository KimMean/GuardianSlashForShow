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
     * ���� �� ��ȭ�� �����մϴ�.
     * Command / Payment / Token / ProductID { / transactionID / receipt }
     */
    /// <summary>
    /// �������� �����մϴ�.
    /// ��Ŷ ũ�� / ��� / ���� ��� / ��ū / ��ǰ ID / {+ �ĺ��ڵ� / ������ }
    /// </summary>
    /// <param name="payment"></param>
    /// <param name="token"></param>
    /// <param name="productID"></param>
    /// <param name="transactionID"></param>
    /// <param name="receipt"></param>
    /// <returns></returns>
    public static ArraySegment<byte> GetPurchaseRequest(Payment payment, string token, string productID, string transactionID = null, string receipt = null)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// ���� ����� �޽��ϴ�.
    /// ��ȯ ��� / ��ǰ ����
    /// ��ȭ ���Ž� + ���� , ���̾�
    /// ����, �����, ���� ���Ž� + ������ �ڵ� ����Ʈ�� ���� ����
    /// </summary>
    public bool ReceivePurchaseResponse(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
}
