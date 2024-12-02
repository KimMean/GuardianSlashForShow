using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBox : MonoBehaviour
{
    const int MAX_LEVEL = 30;

    WeaponPanelController _WeaponPanelController;

    public string ItemCode;

    WeaponDetail _WeaponDetail;
    [SerializeField] Image Image_Weapon;
    [SerializeField] Slider Slider_Quantity;
    [SerializeField] Text Text_Quantity;
    [SerializeField] GameObject Locked;


    private void OnEnable()
    {
    }
    private void OnDestroy()
    {
        if (_WeaponDetail != null)
        {
            _WeaponDetail.OnNewItemAcquired -= OnNewItemAcquired;
            _WeaponDetail.OnUpdateItem -= RefreshQuantity;
        }
    }
    public void SetWeaponPanelController(WeaponPanelController weaponPanelController)
    {
        _WeaponPanelController = weaponPanelController;
    }

    public void SetWeaponData(string code) 
    {
        ItemCode = code;

        _WeaponDetail = WeaponManager.Instance.GetWeaponData(code);
        _WeaponDetail.OnNewItemAcquired += OnNewItemAcquired;
        _WeaponDetail.OnUpdateItem += RefreshQuantity;

        Image_Weapon.sprite = _WeaponDetail.GetWeaponSprite();
        Locked.SetActive(!_WeaponDetail.GetIsPossess());
        RefreshQuantity();
    }

    public void RefreshQuantity()
    {
        //Debug.Log("RefreshQuantity");
        if(_WeaponPanelController != null)
        {
            _WeaponPanelController.UpdateEquipmentView();

        }

        if (_WeaponDetail.GetWeaponLevel() == MAX_LEVEL)
        {
            Slider_Quantity.value = 1;
            Text_Quantity.text = "MAX";
        }
        else
        {
            int quantity = _WeaponDetail.GetWeaponQuantity();
            int EnhancementQuantity = _WeaponDetail.GetEnhancementQuantity();
            float ratio = quantity / (float)EnhancementQuantity;
            Slider_Quantity.value = ratio;
            Text_Quantity.text = quantity.ToString() + " / " + EnhancementQuantity.ToString();
            //Debug.Log("¿Ö ¾Èµé¾î¿È?");
        }
    }

    public void OnWeaponBoxClicked()
    {
        if (_WeaponDetail.GetIsPossess())
        {
            _WeaponPanelController.ShowWeaponDetailPopup(ItemCode);
        }
    }

    public void OnNewItemAcquired()
    {
        //Debug.Log("OnNewItemAcquired");
        if(Locked.activeSelf)
            Locked.SetActive(false);

        RefreshQuantity();
    }
}
