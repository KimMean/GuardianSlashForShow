using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Packet;

public class StagePacket
{
    /// <summary>
    /// Ŭ������ �ִ� ���������� ��û�մϴ�.
    /// ��Ŷ ũ�� / ��� / ��ū ������ / ��ū
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static ArraySegment<byte> GetUserClearStageRequest(string token)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// Ŭ������ ���������� ���޹޽��ϴ�.
    /// ��ȯ ��� / Ŭ������ ��������
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool ReceiveUserClearStageResponse(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// ���� ����� �����մϴ�.
    /// ��Ŷ ũ�� / ��� / ��ū ������ / ��ū / ���� ��� / �������� / ȹ���� ���� / ȹ���� ���̾�
    /// </summary>
    /// <param name="token">��ū</param>
    /// <param name="state">���� ���</param>
    /// <param name="stage">������ ��������</param>
    /// <param name="coin">ȹ���� ����</param>
    /// <param name="diamond">ȹ���� ���̾�</param>
    /// <returns></returns>
    public static ArraySegment<byte> GetStageResultRequest(string token, GameState state, int stage, int coin, int diamond)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }

    /// <summary>
    /// �������� ����� ���޹޽��ϴ�.
    /// ��ȯ ��� / ���� ��� / �������� / ������ ���� / ������ ���̾�
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public bool ReceiveStageResultResponse(ArraySegment<byte> buffer)
    {
                // ������ ��Ŷ ���� ������ ���� �����մϴ�.
    }
}
