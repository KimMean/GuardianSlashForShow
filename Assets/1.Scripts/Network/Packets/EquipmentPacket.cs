using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class EquipmentPacket
{
    /// <summary>
    /// ������ ������ ������ ��û�մϴ�.
    /// ��Ŷ ũ�� / ��� / ��ū ������ / ��ū
    /// </summary>
    /// <param name="token">��ū</param>
    public static ArraySegment<byte> GetUserEquipmentDataRequest(string token)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }
    /// <summary>
    /// ������ ������ ������ �޽��ϴ�.
    /// ��ȯ ��� / ���� �ڵ� ������ / ���� �ڵ� / ����� �ڵ� ������ / ����� �ڵ� / ���� �ڵ� ������ / ���� �ڵ�
    /// </summary>
    public bool ReceiveUserEquipmentDataResponse(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// ������ �������� ������ ��û�մϴ�.
    /// ��Ŷ ũ�� / ��� / ��ū ������ / ��ū / ������ ���� / ������ �ڵ� ������ / ������ �ڵ�
    /// </summary>
    /// <param name="token">��ū</param>
    /// <param name="product">����, �����, ����</param>
    /// <param name="itemCode">������ �ڵ�</param>
    /// <returns></returns>
    public static ArraySegment<byte> ChangeEquipmentRequest(string token, Products product, string itemCode)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// ������ ������ ������ �޽��ϴ�.
    /// ��ȯ ��� / ������ ���� / �ڵ� ������ / ������ �ڵ�
    /// </summary>
    public bool ReceiveChangeEquipmentDataResponse(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
        
    }
}
