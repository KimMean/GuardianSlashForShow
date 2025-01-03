using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDetailPopup : MonoBehaviour
{
    const int MAX_LEVEL = 30;

    string itemCode;
    WeaponDetail _WeaponDetail;

    [SerializeField] Image Image_Weapon;

    [Header("Quantity Slider")]
    [SerializeField] Slider Slider_Quantity;
    [SerializeField] Text Text_Quantity;

    [SerializeField] Text Text_ItemName;
    [SerializeField] Text Text_Level;
    [SerializeField] Text Text_Ability;
    [SerializeField] Text Text_EnhancementPrice;

    private void OnDisable()
    {
        if(_WeaponDetail != null)
            _WeaponDetail.OnUpdateItem -= UpdateItemInformation;

        gameObject.SetActive(false);
    }

    public void SetWeaponData(string code)
    {
        gameObject.SetActive(true);
        itemCode = code;
        _WeaponDetail = WeaponManager.Instance.GetWeaponData(itemCode);
        //Debug.Log($"WeaponDetailPopup SetWeaponData : {itemCode}");
        _WeaponDetail.OnUpdateItem += UpdateItemInformation;

        Text_ItemName.text = _WeaponDetail.GetWeaponName();
        Image_Weapon.sprite = _WeaponDetail.GetWeaponSprite();
        Text_Level.text = _WeaponDetail.GetWeaponLevel().ToString();
        Text_Ability.text = _WeaponDetail.GetAttackPower().ToString();
        Text_EnhancementPrice.text = _WeaponDetail.GetEnhancementPrice().ToString();
        RefreshQuantity();
    }

    /// <summary>
    /// 아이템 수량을 갱신합니다.
    /// </summary>
    public void RefreshQuantity()
    {
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
        }
    }
    /// <summary>
    /// 아이템 정보를 갱신합니다.
    /// </summary>
    public void UpdateItemInformation()
    {
        Text_Level.text = _WeaponDetail.GetWeaponLevel().ToString();
        Text_Ability.text = _WeaponDetail.GetAttackPower().ToString();
        Text_EnhancementPrice.text = _WeaponDetail.GetEnhancementPrice().ToString();

        RefreshQuantity();
    }
    
    /// <summary>
    /// 닫기 버튼 클릭
    /// </summary>
    public void OnClosedButtonClick()
    {
        gameObject.SetActive(false);
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupClose);
    }

    /// <summary>
    /// 장착 버튼 클릭
    /// </summary>
    public void OnEquipmentButtonClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (GameManager.Instance.GetEquipmentWeapon() == itemCode)
            return;

        if (!NetworkManager.Instance.GetIsConnected()) return;

        NetworkManager.Instance.ChangeEquipment(Packet.Products.Weapon, itemCode);
        //GameManager.Instance.SetEquipmentWeapon(WeaponCode);
    }

    /// <summary>
    /// 강화 버튼 클릭
    /// </summary>
    public void OnEnhancementButtonClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (!_WeaponDetail.CanEnhancement()) return;

        if (!NetworkManager.Instance.GetIsConnected()) return;

        NetworkManager.Instance.WeaponEnhancement(itemCode, _WeaponDetail.GetEnhancementQuantity(), _WeaponDetail.GetEnhancementPrice());
        
        //WeaponManager.Instance.EnhancementItem(WeaponCode);
    }
}
