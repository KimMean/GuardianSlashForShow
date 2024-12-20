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

    /// <summary>
    /// 보유한 무기의 수량을 갱신합니다.
    /// </summary>
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
            //Debug.Log("왜 안들어옴?");
        }
    }

    /// <summary>
    /// 아이템 박스 클릭
    /// </summary>
    public void OnWeaponBoxClicked()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (_WeaponDetail.GetIsPossess())
        {
            _WeaponPanelController.ShowWeaponDetailPopup(ItemCode);
        }
    }

    /// <summary>
    /// 아이템을 추가합니다.
    /// </summary>
    public void OnNewItemAcquired()
    {
        //Debug.Log("OnNewItemAcquired");
        if(Locked.activeSelf)
            Locked.SetActive(false);

        RefreshQuantity();
    }
}
