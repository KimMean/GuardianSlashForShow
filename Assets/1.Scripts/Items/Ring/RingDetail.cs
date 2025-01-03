﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RingDetail
{
    string Name;
    /// <summary>
    /// 추가 공격력
    /// </summary>
    int Attack = 0;
    /// <summary>
    /// 추가 골드
    /// </summary>
    int Gold = 0; 
    /// <summary>
    /// 추가 점프력
    /// </summary>
    int Jump = 0;
    bool IsHave = false;
    Sprite RingSprite = null;

    public event Action OnNewItemAcquired;

    public string GetRingName() { return Name; }
    public void SetRingName(string name) { Name = name; }

    public int GetAdditionalAttackPowerRatio() { return Attack; }
    public void SetAdditionalAttackPowerRatio(int attack) { Attack = attack; }

    public int GetGold() { return Gold; }
    public void SetGold(int gold) { Gold = gold; }

    public int GetJump() { return Jump; }
    public void SetJump(int jump) { Jump = jump; }

    public bool GetIsPossess() { return IsHave; }
    public void SetIsPossess(bool have) { IsHave = have; }

    public void SetRingSprite(Sprite ringSprite) { RingSprite = ringSprite; }
    public Sprite GetRingSprite() { return RingSprite; }

    /// <summary>
    /// 보유한 아이템을 추가합니다.
    /// </summary>
    public void AddItem()
    {
        if (!IsHave)
        {
            IsHave = true;
            if (OnNewItemAcquired != null)
                MainThreadDispatcher.Instance.Enqueue(OnNewItemAcquired.Invoke);
        }
    }

}
