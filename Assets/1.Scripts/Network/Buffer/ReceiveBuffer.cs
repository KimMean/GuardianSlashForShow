using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveBuffer
{

    ArraySegment<byte> RecvBuffer;
    int ReadPosition;
    int WritePosition;

    public int DataSize => WritePosition - ReadPosition;
    public int FreeSize => RecvBuffer.Count - WritePosition;

    public ArraySegment<byte> ReadSegment => new ArraySegment<byte>(RecvBuffer.Array, RecvBuffer.Offset + ReadPosition, DataSize);
    public ArraySegment<byte> WriteSegment => new ArraySegment<byte>(RecvBuffer.Array, RecvBuffer.Offset + WritePosition, FreeSize);


    public ReceiveBuffer(int bufferSize)
    {
        RecvBuffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
    }

    public void Clean()
    {
        int dataSize = DataSize;
        if (dataSize == 0)
        {
            // 남은 데이터가 없음
            ReadPosition = WritePosition = 0;
            //Debug.Log("Clean");
        }
        else
        {
            Array.Copy(RecvBuffer.Array, RecvBuffer.Offset + ReadPosition, RecvBuffer.Array, RecvBuffer.Offset, dataSize);
            ReadPosition = 0;
            WritePosition = dataSize;

            //Debug.Log($"ReadPosition : {ReadPosition}, WritePosition : {WritePosition}");
        }
    }

    public bool OnRead(int numOfBytes)
    {
        //Debug.Log($"OnRead numOfBytes: {numOfBytes}, DataSize : {DataSize}");
        if (numOfBytes > DataSize)
            return false;

        ReadPosition += numOfBytes;
        return true;
    }

    public bool OnWrite(int numOfBytes)
    {
        if (numOfBytes > FreeSize)
            return false;

        WritePosition += numOfBytes;
        return true;
    }

}
