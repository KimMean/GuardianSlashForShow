using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NecklaceManager
{
    private static NecklaceManager instance;
    public static NecklaceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NecklaceManager();
            }
            return instance;
        }
    }

    public Dictionary<string, NecklaceDetail> Necklaces = new Dictionary<string, NecklaceDetail>();

    public void SetNecklaceData(string code, string name, int twilight, int _void, int hell)
    {

        Necklaces[code] = new NecklaceDetail();
        Necklaces[code].SetNecklaceName(name);
        Necklaces[code].SetTwilight(twilight);
        Necklaces[code].SetVoid(_void);
        Necklaces[code].SetHell(hell);

        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            Necklaces[code].SetNecklaceSprite(Resources.Load<Sprite>("Necklace/" + code));
        });
        //Debug.Log($"Necklace Data Code : {code}, Name : {name}, Twilight : {twilight}, Void : {_void}, Hell : {hell}");
    }

    public NecklaceDetail GetNecklaceData(string code)
    {
        return Necklaces[code];
    }

    public List<string> GetNecklaceCodeList()
    {
        return new List<string>(Necklaces.Keys);
    }
    public List<string> GetNecklaceNameList()
    {
        List<string> sortedNames = Necklaces
            .OrderBy(necklace => necklace.Key)
            .Select(necklace => necklace.Value.GetNecklaceName())
            .ToList();
        return sortedNames;
    }

    public void AddItems(List<string> itemCodes)
    {
        foreach (string itemCode in itemCodes)
        {
            Necklaces[itemCode].AddItem();
        }
    }



}
