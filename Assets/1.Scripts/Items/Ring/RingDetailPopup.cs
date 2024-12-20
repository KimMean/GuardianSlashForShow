using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingDetailPopup : MonoBehaviour
{
    string ItemCode;
    RingDetail _RingDetail;

    [SerializeField] Image Image_Ring;

    [SerializeField] Text Text_ItemName;
    [SerializeField] Text Text_Attack;
    [SerializeField] Text Text_Gold;
    [SerializeField] Text Text_Jump;

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
    public void SetRingData(string code)
    {
        gameObject.SetActive(true);
        ItemCode = code;
        _RingDetail = RingManager.Instance.GetRingData(ItemCode);
        Debug.Log($"RingDetailPopup SetRingData : {ItemCode}");
        Text_ItemName.text = _RingDetail.GetRingName();
        Image_Ring.sprite = _RingDetail.GetRingSprite();
        Text_Attack.text = "추가 데미지 : + " + _RingDetail.GetAdditionalAttackPowerRatio().ToString() + "%";
        Text_Gold.text = "추가 코인 : + " + _RingDetail.GetGold().ToString() + "%";
        Text_Jump.text = "점프력 : + " + _RingDetail.GetJump().ToString() + "%"; ;
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

        if (GameManager.Instance.GetEquipmentRing() == ItemCode)
            return;

        if (!NetworkManager.Instance.GetIsConnected()) return;

        NetworkManager.Instance.ChangeEquipment(Packet.Products.Ring, ItemCode);
        //GameManager.Instance.SetEquipmentRing(RingCode);
    }

}
