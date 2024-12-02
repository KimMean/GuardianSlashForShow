using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItemBox : MonoBehaviour
{
    [SerializeField] Image itemImage;


    public void SetWeapon(string itemCode)
    {
        itemImage.sprite = WeaponManager.Instance.GetWeaponData(itemCode).GetWeaponSprite();
    }
}
