using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingPanelController : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField] GameObject EquipLock;
    [SerializeField] Image Image_Item;
    [SerializeField] Text Text_ItemName;
    [SerializeField] Text Text_Attack;
    [SerializeField] Text Text_Gold;
    [SerializeField] Text Text_Jump;
    [SerializeField, ReadOnly] string equipItemCode;

    [Header("Inventory")]
    [SerializeField] GameObject RingBoxPrefabs;
    [SerializeField] Transform RingGridLayout;

    [Header("Popup")]
    [SerializeField] RingDetailPopup _RingDetailPopup;

    private void Awake()
    {
        CreateRingBox();
    }

    private void OnEnable()
    {
        UpdateEquipmentView();
        GameManager.Instance.OnChangedEquippedRing += UpdateEquipmentView;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnChangedEquippedRing -= UpdateEquipmentView;
        }
    }


    public void ShowRingDetailPopup(string itemCode)
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupOpen);
        _RingDetailPopup.SetRingData(itemCode);
    }

    /// <summary>
    /// ���ο� �������� ������ ��� ������ �����մϴ�.
    /// </summary>
    void UpdateEquipmentView()
    {
        equipItemCode = GameManager.Instance.GetEquipmentRing();

        if (equipItemCode == "R000")
        {
            Text_ItemName.text = "������ ������";
            Text_Attack.text = "�߰� ������ : + 0%";
            Text_Gold.text = "�߰� ���� : + 0%";
            Text_Jump.text = "������ : + 0%";
            Image_Item.sprite = null;
            EquipLock.SetActive(true);
            return;
        }
        RingDetail itemInfo = RingManager.Instance.GetRingData(equipItemCode);
        Text_ItemName.text = itemInfo.GetRingName();
        Text_Attack.text = "�߰� ������ : + " + itemInfo.GetAdditionalAttackPowerRatio().ToString() + "%";
        Text_Gold.text = "�߰� ���� : + " + itemInfo.GetGold().ToString() + "%";
        Text_Jump.text = "������ : + " + itemInfo.GetJump().ToString() + "%";
        Image_Item.sprite = itemInfo.GetRingSprite();
        EquipLock.SetActive(false);
    }

    void CreateRingBox()
    {
        List<string> codes = RingManager.Instance.GetRingCodeList();

        for (int i = 0; i < codes.Count; i++)
        {
            GameObject obj = Instantiate(RingBoxPrefabs, RingGridLayout);
            //RingManager.Instance.SetRingSprite(codes[i], Resources.Load<Sprite>("Sword/" + codes[i]));
            obj.GetComponent<RingBox>().SetRingData(codes[i]);
            obj.GetComponent<RingBox>().SetRingPanelController(this);
        }
    }
}
