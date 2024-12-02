using System;
using UnityEngine;

public class WeaponDetail
{
    const int MAX_LEVEL = 30;
    const int BASE_UPGRADE_COST = 500;

    string Name;
    int Level = 0;
    int Quantity = 0;
    int AttackLevel = 0;
    Sprite WeaponSprite = null;

    bool IsHave = false;

    public event Action OnNewItemAcquired;
    public event Action OnUpdateItem;

    public string GetWeaponName()
    {
        return Name;
    }

    public void SetWeaponName(string name)
    {
        Name = name;
    }
    public void SetWeaponLevel(int level)
    {
        Level = level;
    }
    public void SetWeaponQuantity(int quantity)
    {
        Quantity = quantity;
    }

    public void SetWeaponAttackLevel(int attackLevel)
    {
        AttackLevel = attackLevel;
    }
    public void SetWeaponSprite(Sprite weaponSprite)
    {
        WeaponSprite = weaponSprite;
    }

    public void SetUserData(int level, int quantity, int attackLevel)
    {
        //Debug.Log("SetUserData Begin");

        IsHave = true;
        Level = level;
        Quantity = quantity;
        AttackLevel = attackLevel;

        if (OnUpdateItem != null)
        {
            //Debug.Log("UpdateItem Invoke");
            MainThreadDispatcher.Instance.Enqueue(OnUpdateItem.Invoke);
            //OnUpdateItem.Invoke();
        }

        //Debug.Log("SetUserData");
    }

    public int GetUpgradeQuantity()
    {
        return Level / 3 + 1;
    }

    public int GetWeaponLevel()
    {
        return Level;
    }
    public int GetWeaponQuantity()
    {
        return Quantity;
    }

    public void AddItemQuantity(int quantity = 1)
    {
        if (!IsHave)
        {
            IsHave = true;
        }
        else
        {
            Quantity += quantity;
        }
        if(OnNewItemAcquired != null)
            MainThreadDispatcher.Instance.Enqueue(OnNewItemAcquired.Invoke);
    }
    public int GetEnhancementQuantity()
    {
        return GetWeaponLevel() / 3 + 1;
    }
    public long GetAttackPower()
    {
        return FibonacciDataManager.Instance.GetFibonacciData(AttackLevel);
    }

    public Sprite GetWeaponSprite()
    {
        return WeaponSprite;
    }

    public bool GetIsPossess()
    {
        return IsHave;
    }

    public int GetEnhancementPrice()
    {
        return AttackLevel * BASE_UPGRADE_COST;
    }

    public bool CanEnhancement()
    {
        if (!IsHave) return false;

        if (Level >= MAX_LEVEL)
            return false;

        if (Quantity < GetUpgradeQuantity())
            return false;

        if (GameManager.Instance.GetCoin() < GetEnhancementPrice())
            return false;

        return true;
    }

    public void Enhancement()
    {
        if (!CanEnhancement()) return;


        // 강화 가능한지 체크

        // 강화에 필요한 것
        // 같은 무기 n 개
        // 강화에 필요한 자금

        // 토큰, 업글코드, 무기 코드, 필요수량, 업글비용 보내기
        // 무기 코드, Level, AttackLevel, 남은 수량, 남은 재화 받기
        Quantity -= GetUpgradeQuantity();
        Level++;
        AttackLevel++;
    }
}
