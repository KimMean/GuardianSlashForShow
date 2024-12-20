using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NecklacePanelController : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField] GameObject EquipLock;
    [SerializeField] Image Image_Item;
    [SerializeField] Text Text_ItemName;
    [SerializeField] Text Text_Twilight;
    [SerializeField] Text Text_Void;
    [SerializeField] Text Text_Hell;
    [SerializeField, ReadOnly] string equipItemCode;

    [Header("Inventory")]
    [SerializeField] GameObject NecklaceBoxPrefabs;
    [SerializeField] Transform NecklaceGridLayout;

    [Header("Popup")]
    [SerializeField] NecklaceDetailPopup _NecklaceDetailPopup;

    private void Awake()
    {
        CreateNecklaceBox();
    }

    private void OnEnable()
    {
        UpdateEquipmentView();
        GameManager.Instance.OnChangedEquippedNecklace += UpdateEquipmentView;
    }
    private void OnDisable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnChangedEquippedNecklace -= UpdateEquipmentView;
        }
    }

    public void ShowNecklaceDetailPopup(string itemCode)
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupOpen);
        _NecklaceDetailPopup.SetNecklaceData(itemCode);
    }

    /// <summary>
    /// ���ο� ����̸� ������ ��� ������ �����մϴ�.
    /// </summary>
    void UpdateEquipmentView()
    {
        equipItemCode = GameManager.Instance.GetEquipmentNecklace();
        if(equipItemCode == "N000")
        {
            Text_ItemName.text = "������ ������";
            Text_Twilight.text = "��� ȸ�� Ȯ�� : + 0%";
            Text_Void.text = "���� �˹� �Ŀ� : + 0%";
            Text_Hell.text = "��� ���� Ȯ�� : + 0%";
            Image_Item.sprite = null;
            EquipLock.SetActive(true);
            return;
        }
        NecklaceDetail itemInfo = NecklaceManager.Instance.GetNecklaceData(equipItemCode);
        Text_ItemName.text = itemInfo.GetNecklaceName();
        Text_Twilight.text = "��� ȸ�� Ȯ�� : + " + itemInfo.GetTwilight().ToString() + "%";
        Text_Void.text = "���� �˹� �Ŀ� : + " + itemInfo.GetVoid().ToString() + "%";
        Text_Hell.text = "��� ���� Ȯ�� : + " + itemInfo.GetHell().ToString() + "%";
        Image_Item.sprite = itemInfo.GetNecklaceSprite();
        EquipLock.SetActive(false);
    }

    void CreateNecklaceBox()
    {
        List<string> codes = NecklaceManager.Instance.GetNecklaceCodeList();

        for (int i = 0; i < codes.Count; i++)
        {
            GameObject obj = Instantiate(NecklaceBoxPrefabs, NecklaceGridLayout);
            //NecklaceManager.Instance.SetNecklaceSprite(codes[i], Resources.Load<Sprite>("Sword/" + codes[i]));
            obj.GetComponent<NecklaceBox>().SetNecklaceData(codes[i]);
            obj.GetComponent<NecklaceBox>().SetNecklacePanelController(this);
        }
    }
}
