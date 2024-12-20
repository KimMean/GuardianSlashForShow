using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using static Packet;

/// <summary>
/// 아이템 확률을 표시합니다.
/// </summary>
public class ItemProbabilityTable : MonoBehaviour
{
    public enum Item
    {
        Weapon,
        Necklace,
        Ring,
    }

    [SerializeField] Item item;
    [SerializeField] GameObject weaponTable;
    [SerializeField] GameObject necklaceTable;
    [SerializeField] GameObject ringTable;
    [SerializeField] GameObject itemProbabilityPrefab;

    private void Awake()
    {
        InitWeaponProbabilityTable();
        InitNecklaceProbabilityTable();
        InitRingProbabilityTable();
    }
    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
    public void ShowProbabilityTable(Products product)
    {
        weaponTable.SetActive(false);
        necklaceTable.SetActive(false);
        ringTable.SetActive(false);

        switch (product)
        {
            case Products.Weapon:
                weaponTable.SetActive(true);
                break;
            case Products.Necklace:
                necklaceTable.SetActive(true);
                break;
            case Products.Ring:
                ringTable.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 무기 아이템 상자별 확률을 계산합니다.
    /// </summary>
    void InitWeaponProbabilityTable()
    {
        List<string> weaponNames = WeaponManager.Instance.GetWeaponNameList();
        int itemCount = 20; // 아이템 개수
        
        // 나무, 은, 금 상자 확률 계산
        double[] woodBox = CalculateProbabilities(30.0, 0.1, itemCount);
        double[] silverBox = CalculateProbabilities(25.0, 0.5, itemCount);
        double[] goldBox = CalculateProbabilities(20.0, 1.0, itemCount);


        for (int i = 0; i < weaponNames.Count; i++)
        {
            GameObject obj = Instantiate(itemProbabilityPrefab, weaponTable.transform);
            ProbabilityTuple tuple = obj.GetComponent<ProbabilityTuple>();
            tuple.SetTuple(weaponNames[i], woodBox[i].ToString("F2"), silverBox[i].ToString("F2"), goldBox[i].ToString("F2"));
        }
        weaponTable.SetActive(false);
    }

    /// <summary>
    /// 목걸이 아이템 상자별 확률을 계산합니다.
    /// </summary>
    void InitNecklaceProbabilityTable()
    {
        List<string> necklaceNames = NecklaceManager.Instance.GetNecklaceNameList();
        int itemCount = 15; // 아이템 개수

        // 나무, 은, 금 상자 확률 계산
        double[] woodBox = CalculateProbabilities(30.0, 0.1, itemCount);
        double[] silverBox = CalculateProbabilities(25.0, 0.5, itemCount);
        double[] goldBox = CalculateProbabilities(20.0, 1.0, itemCount);

        for (int i = 0; i < necklaceNames.Count; i++)
        {
            GameObject obj = Instantiate(itemProbabilityPrefab, necklaceTable.transform);
            ProbabilityTuple tuple = obj.GetComponent<ProbabilityTuple>();
            tuple.SetTuple(necklaceNames[i], woodBox[i].ToString("F2"), silverBox[i].ToString("F2"), goldBox[i].ToString("F2"));
        }
        necklaceTable.SetActive(false);
    }
    /// <summary>
    /// 반지 아이템 상자별 확률을 계산합니다.
    /// </summary>
    void InitRingProbabilityTable()
    {
        List<string> ringNames = RingManager.Instance.GetRingNameList();
        int itemCount = 15; // 아이템 개수

        // 나무, 은, 금 상자 확률 계산
        double[] woodBox = CalculateProbabilities(30.0, 0.1, itemCount);
        double[] silverBox = CalculateProbabilities(25.0, 0.5, itemCount);
        double[] goldBox = CalculateProbabilities(20.0, 1.0, itemCount);

        for (int i = 0; i < ringNames.Count; i++)
        {
            GameObject obj = Instantiate(itemProbabilityPrefab, ringTable.transform);
            ProbabilityTuple tuple = obj.GetComponent<ProbabilityTuple>();
            tuple.SetTuple(ringNames[i], woodBox[i].ToString("F2"), silverBox[i].ToString("F2"), goldBox[i].ToString("F2"));
        }
        ringTable.SetActive(false);
    }

    /// <summary>
    /// 아이템 확률을 계산하는 메서드
    /// </summary>
    /// <param name="startProb">첫 번째 아이템 확률(%)</param>
    /// <param name="endProb">마지막 아이템 확률(%)</param>
    /// <param name="itemCount">아이템 개수</param>
    /// <returns>정규화된 확률 배열</returns>
    public double[] CalculateProbabilities(double startProb, double endProb, int itemCount)
    {
        // 로그 스케일로 확률 계산
        double logStart = Math.Log10(startProb);
        double logEnd = Math.Log10(endProb);
        double[] logProbs = Enumerable.Range(0, itemCount)
                                       .Select(i => logStart + (logEnd - logStart) * i / (itemCount - 1))
                                       .ToArray();

        // 로그 값을 일반 확률 값으로 변환 및 정규화
        double[] probabilities = logProbs.Select(x => Math.Pow(10, x)).ToArray();
        double total = probabilities.Sum();
        return probabilities.Select(p => (p / total) * 100).ToArray();
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }
}
