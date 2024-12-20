using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FibonacciDataManager : MonoBehaviour
{
    private static FibonacciDataManager instance;
    public static FibonacciDataManager Instance => instance;

    [SerializeField] TextAsset csvFile;

    Dictionary<int, long> fibonacciData;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        fibonacciData = new Dictionary<int, long>();
        DataGenerator();
        //LoadCSV();
    }

    /// <summary>
    /// 피보나치 수열을 생성합니다.
    /// </summary>
    private void DataGenerator()
    {
        fibonacciData[0] = 1;
        fibonacciData[1] = 2;
        for(int i = 2; i <= 90; i++)
        {
            fibonacciData.Add(i, fibonacciData[i - 2] + fibonacciData[i - 1]);
        }
    }


    public long GetFibonacciData(int fibonacciIndex)
    {
        return fibonacciData[fibonacciIndex];
    }

    void LoadCSV()
    {
        StringReader reader = new StringReader(csvFile.text);

        bool isFirstLine = true;
        while (reader.Peek() > -1)
        {
            var line = reader.ReadLine();
            if (isFirstLine)
            {
                isFirstLine = false;
                continue; // 첫 번째 줄은 헤더로 무시
            }

            var values = line.Split(',');
            int index = int.Parse(values[0]);
            long value = long.Parse(values[1]);
            fibonacciData.Add(index, value);
        }
    }
}
