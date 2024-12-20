using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NecklaceDetail
{

    string Name;
    /// <summary>
    /// 기사회생 확률
    /// </summary>
    int Twilight = 0;
    /// <summary>
    /// 막기 넉백 파워
    /// </summary>
    int mVoid = 0;
    /// <summary>
    /// 블록 감속 확률
    /// </summary>
    int Hell = 0;
    bool IsHave = false;
    Sprite NecklaceSprite = null;

    public event Action OnNewItemAcquired;

    public string GetNecklaceName() { return Name; }
    public void SetNecklaceName(string name) { Name = name; }

    public int GetTwilight() { return Twilight; }
    public void SetTwilight(int twilight) { Twilight = twilight; }
    
    public int GetVoid() { return mVoid; }
    public void SetVoid(int _void) { mVoid = _void; }

    public int GetHell() {  return Hell; }
    public void SetHell(int hell) { Hell = hell; }
    public bool GetIsPossess() { return IsHave; }
    public void SetIsPossess(bool have) { IsHave = have; }

    public void SetNecklaceSprite(Sprite necklaceSprite) { NecklaceSprite = necklaceSprite; }
    public Sprite GetNecklaceSprite() { return NecklaceSprite; }

    /// <summary>
    /// 아이템을 추가합니다.
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
