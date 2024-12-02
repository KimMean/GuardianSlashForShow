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

    Dictionary<int, long> FibonacciData;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        FibonacciData = new Dictionary<int, long>();
        DataGenerator();
        //LoadCSV();
    }

    private void DataGenerator()
    {
        FibonacciData[0] = 1;
        FibonacciData[1] = 2;
        for(int i = 2; i <= 90; i++)
        {
            FibonacciData.Add(i, FibonacciData[i - 2] + FibonacciData[i - 1]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //StringBuilder sb = new StringBuilder();
        //foreach(var kvp in FibonacciData)
        //{
        //    sb.Append($"Key : {kvp.Key}, Value : {kvp.Value} \n");
        //}
        //Debug.Log( sb.ToString() );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public long GetFibonacciData(int fibonacciIndex)
    {
        return FibonacciData[fibonacciIndex];
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
            FibonacciData.Add(index, value);
        }
    }
}
