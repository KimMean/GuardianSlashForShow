using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;
using static Packet;

public class SocketManager
{
    object Lock = new object();

    Socket _Socket;

    ReceiveBuffer _ReceiveBuffer = new ReceiveBuffer(2048);

    SocketAsyncEventArgs ReceiveArgs = new SocketAsyncEventArgs();
    SocketAsyncEventArgs SendArgs = new SocketAsyncEventArgs();

    Queue<ArraySegment<byte>> SendQueue = new Queue<ArraySegment<byte>>();
    List<ArraySegment<byte>> SendPendingList = new List<ArraySegment<byte>>();

    public static readonly int HEADER_SIZE = 2;
    public void Init(IPEndPoint endPoint)
    {
        _Socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += OnConnectCompleted;
        args.RemoteEndPoint = endPoint;
        args.UserToken = _Socket;

        RegisterConnect(args);
    }

    void RegisterConnect(SocketAsyncEventArgs args)
    {
        bool pending = _Socket.ConnectAsync(args);
        if (pending == false)
            OnConnectCompleted(null, args);
    }

    public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            OnConnected(args.RemoteEndPoint);
            SessionStart();
            Debug.Log($"OnConnectCompleted : {args.SocketError}");
        }
        else
        {
            Debug.Log($"OnConnectCompleted Fail : {args.SocketError}");
        }
    }

    public void OnConnected(EndPoint remoteEndPoint)
    {
        //Debug.Log($"OnConnected : {remoteEndPoint}");
    }

    public void SessionStart()
    {
        ReceiveArgs.Completed += OnReceiveCompleted;
        SendArgs.Completed += OnSendCompleted;

        RegisterReceive();
        Debug.Log("SessionStart");
    }


    public void Disconnect()
    {
        if(_Socket != null && _Socket.Connected)
        {
            _Socket.Shutdown(SocketShutdown.Both);
            _Socket.Close();
            Debug.Log("Disconnect");
        }
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        lock (Lock)
        {
            SendQueue.Enqueue(sendBuff);

            if (SendPendingList.Count == 0)
                RegisterSend();
        }
    }
    void RegisterSend()
    {
        while (SendQueue.Count > 0)
        {
            ArraySegment<byte> buff = SendQueue.Dequeue();
            //SendArgs.SetBuffer(buff, 0, buff.Length);
            SendPendingList.Add(buff);
        }
        SendArgs.BufferList = SendPendingList;

        bool pending = _Socket.SendAsync(SendArgs);
        if (pending == false)
            OnSendCompleted(null, SendArgs);
    }
    void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            try
            {
                SendArgs.BufferList = null;
                SendPendingList.Clear();

                //Console.WriteLine($"Transferred bytes : {SendArgs.BytesTransferred}");

                OnSend(SendArgs.BytesTransferred);
                if (SendQueue.Count > 0)
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

    void RegisterReceive()
    {
        //Debug.Log($"RegisterReceive : {_ReceiveBuffer}");
        _ReceiveBuffer.Clean();
        ArraySegment<byte> segment = _ReceiveBuffer.WriteSegment;
        ReceiveArgs.SetBuffer(segment);

        bool pending = _Socket.ReceiveAsync(ReceiveArgs);
        if (pending == false)
            OnReceiveCompleted(null, ReceiveArgs);
    }

    void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
    {
        //Debug.Log($"OnReceiveCompleted Byte : {args.BytesTransferred}");
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            // Write Cursor 이동
            if (_ReceiveBuffer.OnWrite(args.BytesTransferred) == false)
            {
                //Debug.Log($"_ReceiveBuffer.OnWrite(args.BytesTransferred) == false {_ReceiveBuffer.OnWrite(args.BytesTransferred) == false}");
                Disconnect();
                return;
            }

            int processLen = OnReceive(_ReceiveBuffer.ReadSegment);
            //Debug.Log($"processLen {processLen}");
            if (_ReceiveBuffer.DataSize < processLen)
            {
                //Debug.Log($"_ReceiveBuffer.DataSize < processLen {_ReceiveBuffer.DataSize < processLen}");
                Disconnect();
                return;
            }
            

            if (_ReceiveBuffer.OnRead(processLen) == false)
            {
                //Debug.Log($"_ReceiveBuffer.OnRead(processLen) == false {_ReceiveBuffer.OnRead(processLen) == false}");
                Disconnect();
                return;
            }
            //Debug.Log($"OnReceiveCompleted !!!!!!!  : {processLen}");
            RegisterReceive();

            //Debug.Log($"OnReceiveCompleted ??????????  : {processLen}");
        }
        else
        {
            //Debug.Log($"args.BytesTransferred > 0 && args.SocketError == SocketError.Success");
            Disconnect();
        }
    }

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
