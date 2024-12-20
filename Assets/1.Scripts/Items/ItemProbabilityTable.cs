using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using static Packet;

/// <summary>
/// ������ Ȯ���� ǥ���մϴ�.
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
    /// ���� ������ ���ں� Ȯ���� ����մϴ�.
    /// </summary>
    void InitWeaponProbabilityTable()
    {
        List<string> weaponNames = WeaponManager.Instance.GetWeaponNameList();
        int itemCount = 20; // ������ ����
        
        // ����, ��, �� ���� Ȯ�� ���
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
    /// ����� ������ ���ں� Ȯ���� ����մϴ�.
    /// </summary>
    void InitNecklaceProbabilityTable()
    {
        List<string> necklaceNames = NecklaceManager.Instance.GetNecklaceNameList();
        int itemCount = 15; // ������ ����

        // ����, ��, �� ���� Ȯ�� ���
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
    /// ���� ������ ���ں� Ȯ���� ����մϴ�.
    /// </summary>
    void InitRingProbabilityTable()
    {
        List<string> ringNames = RingManager.Instance.GetRingNameList();
        int itemCount = 15; // ������ ����

        // ����, ��, �� ���� Ȯ�� ���
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
    /// ������ Ȯ���� ����ϴ� �޼���
    /// </summary>
    /// <param name="startProb">ù ��° ������ Ȯ��(%)</param>
    /// <param name="endProb">������ ������ Ȯ��(%)</param>
    /// <param name="itemCount">������ ����</param>
    /// <returns>����ȭ�� Ȯ�� �迭</returns>
    public double[] CalculateProbabilities(double startProb, double endProb, int itemCount)
    {
        // �α� �����Ϸ� Ȯ�� ���
        double logStart = Math.Log10(startProb);
        double logEnd = Math.Log10(endProb);
        double[] logProbs = Enumerable.Range(0, itemCount)
                                       .Select(i => logStart + (logEnd - logStart) * i / (itemCount - 1))
                                       .ToArray();

        // �α� ���� �Ϲ� Ȯ�� ������ ��ȯ �� ����ȭ
        double[] probabilities = logProbs.Select(x => Math.Pow(10, x)).ToArray();
        double total = probabilities.Sum();
        return probabilities.Select(p => (p / total) * 100).ToArray();
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }
}
