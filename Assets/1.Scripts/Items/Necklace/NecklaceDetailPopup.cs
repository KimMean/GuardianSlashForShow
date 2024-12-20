using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NecklaceDetailPopup : MonoBehaviour
{
    string ItemCode;
    NecklaceDetail _NecklaceDetail;

    [SerializeField] Image Image_Necklace;

    [SerializeField] Text Text_ItemName;
    [SerializeField] Text Text_Twilight;
    [SerializeField] Text Text_Void;
    [SerializeField] Text Text_Hell;

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 아이템 정보를 갱신합니다.
    /// </summary>
    /// <param name="code">아이템 코드</param>
    public void SetNecklaceData(string code)
    {
        gameObject.SetActive(true);
        ItemCode = code;
        _NecklaceDetail = NecklaceManager.Instance.GetNecklaceData(ItemCode);
        Debug.Log($"NecklaceDetailPopup SetNecklaceData : {ItemCode}");
        Text_ItemName.text = _NecklaceDetail.GetNecklaceName();
        Image_Necklace.sprite = _NecklaceDetail.GetNecklaceSprite();
        Text_Twilight.text = "기사 회생 확률 : + " + _NecklaceDetail.GetTwilight().ToString() + "%";
        Text_Void.text = "막기 넉백 파워 : + " + _NecklaceDetail.GetVoid().ToString() + "%";
        Text_Hell.text = "블록 감속 확률 : + " + _NecklaceDetail.GetHell().ToString() + "%";
    }

    /// <summary>
    /// 닫기 버튼 클릭
    /// </summary>
    public void OnClosedButtonClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupClose);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 장착 버튼 클릭
    /// </summary>
    public void OnEquipmentButtonClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (GameManager.Instance.GetEquipmentNecklace() == ItemCode)
            return;

        if (!NetworkManager.Instance.GetIsConnected()) return;

        NetworkManager.Instance.ChangeEquipment(Packet.Products.Necklace, ItemCode);
    }
}
