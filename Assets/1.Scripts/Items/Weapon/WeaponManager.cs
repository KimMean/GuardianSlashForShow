using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WeaponManager
{
    private static WeaponManager instance;
    public static WeaponManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WeaponManager();
            }
            return instance;
        }
    }

    public Dictionary<string, WeaponDetail> Weapons = new Dictionary<string, WeaponDetail>();

    public void SetWeaponData(string code, string name, int attackLevel)
    {

        Weapons[code] = new WeaponDetail();
        Weapons[code].SetWeaponName(name);
        Weapons[code].SetWeaponAttackLevel(attackLevel);
        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            Weapons[code].SetWeaponSprite(Resources.Load<Sprite>("Sword/" + code));
        });
        //Debug.Log($"Weapon Data Code : {code}, Name : {name}, AtkLevel : {attackLevel}");
    }

    public void SetUserWeaponData(string code, int enhancementLevel, int quantity, int attackLevel)
    {
        //Debug.Log($"Weapon Data code : {Weapons[code]}");
        //Debug.Log($"Weapon Data Code : {code}, EnhancementLevel : {enhancementLevel}, Quantity : {quantity}, AtkLevel : {attackLevel}");
        Weapons[code].SetUserData(enhancementLevel, quantity, attackLevel);
    }

    public WeaponDetail GetWeaponData(string code)
    {
        //Debug.Log($"GetWeaponData : {code}");
        return Weapons[code];
    }

    public List<string> GetWeaponCodeList()
    {
        return new List<string>(Weapons.Keys.OrderBy(k => k));
    }

    public List<string> GetWeaponNameList()
    {
        List<string> sortedNames = Weapons
            .OrderBy(weapon => weapon.Key)
            .Select(weapon => weapon.Value.GetWeaponName())
            .ToList();
        return sortedNames;
    }

    public void AddItems(List<string> itemCodes)
    {
        foreach (string itemCode in itemCodes)
        {
            Weapons[itemCode].AddItemQuantity();
        }
    }

    //public void SetUserWeaponData(string code, int level, int quantity, int attackLevel)
    //{
    //    Weapons[code].SetUserData(level, quantity, attackLevel);
    //    Debug.Log($"Weapon Data Code : {code}, Level : {level}, Quantity : {quantity}, AtkLevel : {attackLevel}");
    //}
}
