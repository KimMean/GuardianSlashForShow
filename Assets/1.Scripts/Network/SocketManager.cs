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
    bool isConnecting = false; // 연결 시도 중

    ReceiveBuffer receiveBuffer = new ReceiveBuffer(2048);

    SocketAsyncEventArgs receiveArgs;
    SocketAsyncEventArgs sendArgs;

    Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();
    List<ArraySegment<byte>> sendPendingList = new List<ArraySegment<byte>>();

    /// <summary>
    /// 소켓을 초기화 합니다.
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
    /// 소켓을 비동기로 연결합니다.
    /// 즉시 연결된 경우 처리합니다.
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
    /// 연결 응답을 받습니다.
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
    /// 연결된 경우 이벤트를 받습니다. 현재 사용되지 않습니다.
    /// </summary>
    /// <param name="remoteEndPoint"></param>
    public void OnConnected(EndPoint remoteEndPoint)
    {
        //Debug.Log($"OnConnected : {remoteEndPoint}");
    }

    /// <summary>
    /// 세션을 시작하고 데이터 송수신 준비를 합니다.
    /// </summary>
    public void SessionStart()
    {
        receiveArgs.Completed += OnReceiveCompleted;
        sendArgs.Completed += OnSendCompleted;

        RegisterReceive();
        Debug.Log("SessionStart");
    }

    /// <summary>
    /// 소켓이 연결되었는지 확인합니다.
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
            return false; // 연결 문제 발생
        }
    }

    /// <summary>
    /// 연결을 종료합니다.
    /// </summary>
    public void Disconnect()
    {
        if(socket != null && isConnected)
        {
            // 소켓이 실제로 연결되어 있는지 확인
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
        // 재연결 시도
        //StartReconnect();
    }


    public bool GetIsConnected()
    {
        return isConnected;
    }

    /// <summary>
    /// 데이터를 송신합니다.
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
    /// 큐에 있는 데이터를 전송합니다.
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
    /// 전송이 완료된 경우 호출됩니다.
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
    /// 데이터를 수신합니다.
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
    /// 데이터가 수신된 경우 호출됩니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            // Write Cursor 이동
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
    /// 전송받은 데이터를 1차 검증합니다.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    int OnReceive(ArraySegment<byte> buffer)
    {
        int processLength = 0;

        while (true)
        {
            // 최소한 헤더는 파싱할 수 있는지 확인
            if (buffer.Count < HEADER_SIZE)
            {
                //Debug.Log($"ERROR!!!!!!!! buffer.Count : {buffer.Count} < HEADER_SIZE : {HEADER_SIZE}");
                break;
            }

            // 헤더에 담긴 데이터 사이즈 확인
            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            //Debug.Log($"Data Size : {dataSize}, Buffer.Count : {buffer.Count}");
            if (buffer.Count < dataSize)
                break;

            // 패킷 조립 가능
            PacketManager.Instance.OnReceivePacket(new ArraySegment<byte>(buffer.Array, buffer.Offset + HEADER_SIZE, dataSize - HEADER_SIZE));


            processLength += dataSize;
            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }

        return processLength;
    }

   
}
