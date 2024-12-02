using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RingManager
{
    private static RingManager instance;
    public static RingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RingManager();
            }
            return instance;
        }
    }

    public Dictionary<string, RingDetail> Rings = new Dictionary<string, RingDetail>();

    public void SetRingData(string code, string name, int attack, int gold, int jump)
    {

        Rings[code] = new RingDetail();
        Rings[code].SetRingName(name);
        Rings[code].SetAdditionalAttackPowerRatio(attack);
        Rings[code].SetGold(gold);
        Rings[code].SetJump(jump);

        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            Rings[code].SetRingSprite(Resources.Load<Sprite>("Ring/" + code));
        });
        //Debug.Log($"Ring Data Code : {code}, Name : {name}, Twilight : {attack}, Void : {gold}, Hell : {jump}");
    }

    public RingDetail GetRingData(string code)
    {
        return Rings[code];
    }

    public List<string> GetRingCodeList()
    {
        return new List<string>(Rings.Keys);
    }

    public List<string> GetRingNameList()
    {
        List<string> sortedNames = Rings
            .OrderBy(ring => ring.Key)
            .Select(ring => ring.Value.GetRingName())
            .ToList();
        return sortedNames;
    }

    public void AddItems(List<string> itemCodes)
    {
        foreach (string itemCode in itemCodes)
        {
            Rings[itemCode].AddItem();
        }
    }
}
