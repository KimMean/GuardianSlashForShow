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
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = 4;
        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);
        BitConverter.GetBytes((ushort)Command.GuestSignUP).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));

        return SendBufferHelper.Close(count);
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
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort);
        BitConverter.GetBytes((ushort)command).CopyTo(openSegment.Array, openSegment.Offset + sizeof(ushort));
        count += sizeof(ushort);

        if(command == Command.GoogleSignUP)
        {
            byte[] id = Encoding.UTF8.GetBytes(userId);
            ushort idSize = (ushort)id.Length;

            BitConverter.GetBytes(idSize).CopyTo(openSegment.Array, openSegment.Offset + count);
            count += sizeof(ushort);
            id.CopyTo(openSegment.Array, openSegment.Offset + count);
            count += idSize;
        }

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);
        return SendBufferHelper.Close(count);
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
        Debug.Log("ȸ�� �����");
        int packetHeaderSize = 2;
        // ��� ����
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);
        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        string userid = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, dataSize);
        Debug.Log($"ȸ�� ��� �Ϸ� �� UUID : {userid}");

        if(command == Command.GuestSignUP)
            DataManager.Instance.SetUserUUID(userid);
        else if(command == Command.GoogleSignUP)
            DataManager.Instance.SetUserGoogleID(userid);

        Debug.Log($"ȸ�� ��� �Ϸ� UUID : {userid}");

        return true;
    }

    /// <summary>
    /// �α����� ��û�մϴ�.
    /// ��Ŷ ũ�� / ��� / ���̵� ������ / ���̵�
    /// </summary>
    public static ArraySegment<byte> LoginRequest(Command command, string userId)
    {

        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = sizeof(ushort);

        // Command �Է�
        BitConverter.GetBytes((ushort)command).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        byte[] userID = Encoding.UTF8.GetBytes(userId);
        ushort idSize = (ushort)userID.Length;

        BitConverter.GetBytes(idSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        userID.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += idSize;

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);

        return SendBufferHelper.Close(count);

    }
    /// <summary>
    /// �α��� ����� �޽��ϴ�.
    /// ��ȯ ��� / ������ ũ�� / �׼��� ��ū
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool CheckLoginResponse(ArraySegment<byte> buffer)
    {
        //Debug.Log("�Խ�Ʈ �α���");
        int packetHeaderSize = 2;
        // ��� ����
        if (buffer.Count < packetHeaderSize)
            return false;

        int parsingCount = 0;
        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);
        if ((ResultCommand)result == ResultCommand.Failed)
            return false;

        ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        if (dataSize != ACCESS_TOKEN_SIZE)
            return false;

        string token = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, dataSize);
        DataManager.Instance.SetAccessToken(token);
        //Debug.Log($"�α��� �Ϸ�  : {token}");

        return true;
    }
}
