using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class NecklacePacket
{
    /// <summary>
    /// ������ �����͸� ��û�մϴ�.
    /// ��Ŷ ������ / ���
    /// </summary>
    public static ArraySegment<byte> GetNecklaceDataRequest()
    {
               // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// ���޹��� ������ ������
    /// ��ȯ ��� / ������ ���� / ������ ����(������ŭ) {������ �ڵ�, ������ �̸�, ������ �ɷ�}
    /// </summary>
    public bool ReceiveNecklaceData(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }


    /// <summary>
    /// ����ڰ� ������ ������ �����͸� ��û�մϴ�.
    /// ��Ŷ ũ�� / ��� / AccessTokenũ�� / Token
    /// </summary>
    public static ArraySegment<byte> GetUserNecklaceDataRequest(string token)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// ����ڰ� ������ ������ ������
    /// ��ȯ ��� / ������ ���� / ������ ���� {������ �ڵ�}
    /// </summary>
    public bool ReceiveUserNecklaceData(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

}
