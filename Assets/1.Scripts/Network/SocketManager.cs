using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static Packet;

public class SocketManager
{
    public static readonly int HEADER_SIZE = 2;

    object Lock = new object();

    Socket socket;
    IPEndPoint ipEndPoint;
    bool isConnected = false;
    bool isConnecting = false; // ���� �õ� ��

    ReceiveBuffer receiveBuffer = new ReceiveBuffer(2048);

    SocketAsyncEventArgs receiveArgs;
    SocketAsyncEventArgs sendArgs;

    Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();
    List<ArraySegment<byte>> sendPendingList = new List<ArraySegment<byte>>();

    /// <summary>
    /// ������ �ʱ�ȭ �մϴ�.
    /// </summary>
    /// <param name="endPoint">IPEndPoint</param>
    public void Init(IPEndPoint endPoint)
    {
        if (isConnecting) return;
        if (isConnected) return;

        isConnecting = true;
        ipEndPoint = endPoint;
        socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        receiveArgs = new SocketAsyncEventArgs();
        sendArgs = new SocketAsyncEventArgs();

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += OnConnectCompleted;
        args.RemoteEndPoint = ipEndPoint;
        args.UserToken = socket;
        

        RegisterConnect(args);
    }

    /// <summary>
    /// ������ �񵿱�� �����մϴ�.
    /// ��� ����� ��� ó���մϴ�.
    /// </summary>
    /// <param name="args">SocketAsyncEventArgs</param>
    void RegisterConnect(SocketAsyncEventArgs args)
    {
        //Debug.Log("Try Connect");
        bool pending = socket.ConnectAsync(args);
        //Debug.Log($"ConnectAsync : {pending}");
        if (pending == false)
            OnConnectCompleted(null, args);
    }

    /// <summary>
    /// ���� ������ �޽��ϴ�.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="args">SocketAsyncEventArgs</param>
    public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            OnConnected(args.RemoteEndPoint);
            SessionStart();
            Debug.Log($"OnConnectCompleted : {args.SocketError}");
            isConnected = true;
        }
        else
        {
            Debug.Log($"OnConnectCompleted Fail : {args.SocketError}");
            isConnected = false;
        }
        isConnecting = false;
    }

    /// <summary>
    /// ����� ��� �̺�Ʈ�� �޽��ϴ�. ���� ������ �ʽ��ϴ�.
    /// </summary>
    /// <param name="remoteEndPoint"></param>
    public void OnConnected(EndPoint remoteEndPoint)
    {
        //Debug.Log($"OnConnected : {remoteEndPoint}");
    }

    /// <summary>
    /// ������ �����ϰ� ������ �ۼ��� �غ� �մϴ�.
    /// </summary>
    public void SessionStart()
    {
        receiveArgs.Completed += OnReceiveCompleted;
        sendArgs.Completed += OnSendCompleted;

        RegisterReceive();
        Debug.Log("SessionStart");
    }

    /// <summary>
    /// ������ ����Ǿ����� Ȯ���մϴ�.
    /// </summary>
    /// <returns>isConnected</returns>
    public bool IsSocketConnected()
    {
        try
        {
            return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        }
        catch
        {
            return false; // ���� ���� �߻�
        }
    }

    /// <summary>
    /// ������ �����մϴ�.
    /// </summary>
    public void Disconnect()
    {
        if(socket != null && isConnected)
        {
            // ������ ������ ����Ǿ� �ִ��� Ȯ��
            if (IsSocketConnected())
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    Debug.Log("Socket shutdown sent to server.");
                }
                catch (SocketException ex)
                {
                    Debug.Log($"Socket shutdown failed: {ex.Message}");
                }
            }

            socket.Close();
            Debug.Log("Socket closed.");
            isConnected = false;
        }
        // �翬�� �õ�
        //StartReconnect();
    }


    public bool GetIsConnected()
    {
        return isConnected;
    }

    /// <summary>
    /// �����͸� �۽��մϴ�.
    /// </summary>
    /// <param name="sendBuff"></param>
    public void Send(ArraySegment<byte> sendBuff)
    {
        lock (Lock)
        {
            sendQueue.Enqueue(sendBuff);

            if (sendPendingList.Count == 0)
                RegisterSend();
        }
    }
    /// <summary>
    /// ť�� �ִ� �����͸� �����մϴ�.
    /// </summary>
    void RegisterSend()
    {
        while (sendQueue.Count > 0)
        {
            ArraySegment<byte> buff = sendQueue.Dequeue();
            //sendArgs.SetBuffer(buff, 0, buff.Length);
            sendPendingList.Add(buff);
        }
        sendArgs.BufferList = sendPendingList;

        bool pending = socket.SendAsync(sendArgs);
        if (pending == false)
            OnSendCompleted(null, sendArgs);
    }
    /// <summary>
    /// ������ �Ϸ�� ��� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            try
            {
                sendArgs.BufferList = null;
                sendPendingList.Clear();

                //Console.WriteLine($"Transferred bytes : {sendArgs.BytesTransferred}");

                OnSend(sendArgs.BytesTransferred);
                if (sendQueue.Count > 0)
                    RegisterSend();
            }
            catch (Exception ex)
            {
                Debug.Log($"OnSendCompleted Failed : {ex}");
            }
        }
        else
        {
            Disconnect();
        }
    }

    public void OnSend(int numOfBytes)
    {
        //Debug.Log($"Transferred bytes : {numOfBytes}");
    }

    /// <summary>
    /// �����͸� �����մϴ�.
    /// </summary>
    void RegisterReceive()
    {
        //Debug.Log($"RegisterReceive : {receiveBuffer}");
        receiveBuffer.Clean();
        ArraySegment<byte> segment = receiveBuffer.WriteSegment;
        receiveArgs.SetBuffer(segment);

        bool pending = socket.ReceiveAsync(receiveArgs);
        if (pending == false)
            OnReceiveCompleted(null, receiveArgs);
    }
    /// <summary>
    /// �����Ͱ� ���ŵ� ��� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            // Write Cursor �̵�
            if (receiveBuffer.OnWrite(args.BytesTransferred) == false)
            {
                //Debug.Log($"receiveBuffer.OnWrite(args.BytesTransferred) == false {receiveBuffer.OnWrite(args.BytesTransferred) == false}");
                Disconnect();
                return;
            }

            int processLen = OnReceive(receiveBuffer.ReadSegment);
            //Debug.Log($"processLen {processLen}");
            if (receiveBuffer.DataSize < processLen)
            {
                //Debug.Log($"receiveBuffer.DataSize < processLen {receiveBuffer.DataSize < processLen}");
                Disconnect();
                return;
            }
            

            if (receiveBuffer.OnRead(processLen) == false)
            {
                //Debug.Log($"receiveBuffer.OnRead(processLen) == false {receiveBuffer.OnRead(processLen) == false}");
                Disconnect();
                return;
            }
            //Debug.Log($"OnReceiveCompleted !!!!!!!  : {processLen}");
            RegisterReceive();

            //Debug.Log($"OnReceiveCompleted ??????????  : {processLen}");
        }
        else
        {
            Debug.Log($"args.SocketError : {args.SocketError}");
            Disconnect();
        }
    }

    /// <summary>
    /// ���۹��� �����͸� 1�� �����մϴ�.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    int OnReceive(ArraySegment<byte> buffer)
    {
        int processLength = 0;

        while (true)
        {
            // �ּ��� ����� �Ľ��� �� �ִ��� Ȯ��
            if (buffer.Count < HEADER_SIZE)
            {
                //Debug.Log($"ERROR!!!!!!!! buffer.Count : {buffer.Count} < HEADER_SIZE : {HEADER_SIZE}");
                break;
            }

            // ����� ��� ������ ������ Ȯ��
            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            //Debug.Log($"Data Size : {dataSize}, Buffer.Count : {buffer.Count}");
            if (buffer.Count < dataSize)
                break;

            // ��Ŷ ���� ����
            PacketManager.Instance.OnReceivePacket(new ArraySegment<byte>(buffer.Array, buffer.Offset + HEADER_SIZE, dataSize - HEADER_SIZE));


            processLength += dataSize;
            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }

        return processLength;
    }

   
}
