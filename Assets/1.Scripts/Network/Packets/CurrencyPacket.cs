using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Packet;

public class CurrencyPacket
{

    /// <summary>
    /// ������� ��ȭ ������ ��û�մϴ�.
    /// ��Ŷ ũ�� / ��� / ��ū ������ / ��ū
    /// </summary>
    /// <param name="token">��ū</param>
    public static ArraySegment<byte> GetCurrencyRequest(string token)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// ������� ��ȭ ������ �޽��ϴ�.
    /// ��ȯ ��� / ���� / ���̾�
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool CheckCurrencyResponse(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }
}
