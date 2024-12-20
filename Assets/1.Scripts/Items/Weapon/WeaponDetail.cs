using System;
using UnityEngine;

public class WeaponDetail
{
    const int MAX_LEVEL = 30;
    const int BASE_UPGRADE_COST = 500;

    string Name;
    /// <summary>
    /// 강화 레벨
    /// </summary>
    int Level = 0;
    /// <summary>
    /// 무기 수량
    /// </summary>
    int Quantity = 0;
    /// <summary>
    /// 공격 레벨
    /// </summary>
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

    public int GetWeaponLevel()
    {
        return Level;
    }
    public int GetWeaponQuantity()
    {
        return Quantity;
    }

    /// <summary>
    /// 무기의 수량을 추가합니다.
    /// </summary>
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

    /// <summary>
    /// 강화하는데 필요한 무기 수량
    /// </summary>
    public int GetEnhancementQuantity()
    {
        return GetWeaponLevel() / 3 + 1;
    }
    /// <summary>
    /// 무기의 공격력
    /// </summary>
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

    /// <summary>
    /// 강화 비용
    /// </summary>
    public int GetEnhancementPrice()
    {
        return AttackLevel * BASE_UPGRADE_COST;
    }

    /// <summary>
    /// 강화 가능 여부를 판단합니다.
    /// </summary>
    public bool CanEnhancement()
    {
        if (!IsHave) return false;

        if (Level >= MAX_LEVEL)
        {
            MessageManager.Instance.ShowMessage("최대 레벨입니다.");
            return false;
        }

        if (Quantity < GetEnhancementQuantity())
        {
            MessageManager.Instance.ShowMessage("강화를 위한 수량이 충분하지 않습니다.");
            return false;
        }

        if (GameManager.Instance.GetCoin() < GetEnhancementPrice())
        {
            MessageManager.Instance.ShowMessage("보유한 재화가 부족합니다.");
            return false;
        }

        return true;
    }

    public void Enhancement()
    {
        if (!CanEnhancement()) return;

        Quantity -= GetEnhancementQuantity();
        Level++;
        AttackLevel++;
    }
}
