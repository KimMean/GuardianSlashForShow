using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingItemBox : MonoBehaviour
{
    [SerializeField] Image itemImage;


    public void SetRing(string itemCode)
    {
        itemImage.sprite = RingManager.Instance.GetRingData(itemCode).GetRingSprite();
    }
}
