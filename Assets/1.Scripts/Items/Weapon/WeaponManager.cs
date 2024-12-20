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
    }

    public void SetUserWeaponData(string code, int enhancementLevel, int quantity, int attackLevel)
    {
        Weapons[code].SetUserData(enhancementLevel, quantity, attackLevel);
    }

    public WeaponDetail GetWeaponData(string code)
    {
        return Weapons[code];
    }

    /// <summary>
    /// ���� �ڵ� ����Ʈ�� ��ȯ�մϴ�.
    /// </summary>
    public List<string> GetWeaponCodeList()
    {
        return new List<string>(Weapons.Keys.OrderBy(k => k));
    }

    /// <summary>
    /// ���� �̸� ����Ʈ�� ��ȯ�մϴ�.
    /// </summary>
    public List<string> GetWeaponNameList()
    {
        List<string> sortedNames = Weapons
            .OrderBy(weapon => weapon.Key)
            .Select(weapon => weapon.Value.GetWeaponName())
            .ToList();
        return sortedNames;
    }

    /// <summary>
    /// �������� �߰��մϴ�.
    /// </summary>
    /// <param name="itemCodes"></param>
    public void AddItems(List<string> itemCodes)
    {
        foreach (string itemCode in itemCodes)
        {
            Weapons[itemCode].AddItemQuantity();
        }
    }

}
