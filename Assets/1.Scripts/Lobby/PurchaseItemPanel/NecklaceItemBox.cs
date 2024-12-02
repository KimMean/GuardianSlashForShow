using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NecklaceItemBox : MonoBehaviour
{
    [SerializeField] Image itemImage;


    public void SetNecklace(string itemCode)
    {
        itemImage.sprite = NecklaceManager.Instance.GetNecklaceData(itemCode).GetNecklaceSprite();
    }
}
