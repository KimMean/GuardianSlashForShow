using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NecklaceDetail
{

    string Name;
    int Twilight = 0;
    int mVoid = 0;
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
