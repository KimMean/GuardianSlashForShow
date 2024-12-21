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
    /// ������ ���� ��û
    /// ������ �ʽ��ϴ�.
    /// </summary>
    /// <returns></returns>
    public static ArraySegment<byte> GetRegistrationRequest()
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }
    
    /*
     * 
     */
    /// <summary>
    /// ������ ȸ�� ��� ��û
    /// ������ ũ�� / Ŀ�ǵ� / ���� ID(email @gmail.com)
    /// �Խ�Ʈ�� ��� �������� UUID�� �����Ͽ� ��ȯ�մϴ�.
    /// </summary>
    public static ArraySegment<byte> GetRegistrationRequest(Command command, string userId = null)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// ��� ����� �޽��ϴ�.
    /// ��ȯ ��� / ������ ũ�� / UUID / 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool CheckRegistrationResponse(Command command, ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// �α����� ��û�մϴ�.
    /// ��Ŷ ũ�� / ��� / ���̵� ������ / ���̵�
    /// </summary>
    public static ArraySegment<byte> LoginRequest(Command command, string userId)
    {

                // ������ ��Ŷ ���� ������ ���� �����մϴ�.

    }
    /// <summary>
    /// �α��� ����� �޽��ϴ�.
    /// ��ȯ ��� / ������ ũ�� / �׼��� ��ū
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool CheckLoginResponse(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }
}
