using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanelController : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField] Image Image_Item;
    [SerializeField] Text Text_ItemName;
    [SerializeField] Text Text_Ability;
    private string equipItemCode;
    WeaponDetail _weaponDetail;

    [Header("Inventory")]
    [SerializeField] GameObject WeaponBoxPrefabs;
    [SerializeField] Transform WeaponGridLayout;

    [Header("Popup")]
    [SerializeField] WeaponDetailPopup _WeaponDetailPopup;

    private void Awake()
    {
        CreateWeaponBox();
    }

    private void Start()
    {
        //UpdateEquipmentView();
    }

    private void OnEnable()
    {
        UpdateEquipmentView();
        GameManager.Instance.OnChangedEquippedWeapon += UpdateEquipmentView;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnChangedEquippedWeapon -= UpdateEquipmentView;
        }
    }

    public void ShowWeaponDetailPopup(string itemCode)
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupOpen);
        _WeaponDetailPopup.SetWeaponData(itemCode);
    }

    /// <summary>
    /// 장착한 장비의 정보를 갱신합니다.
    /// </summary>
    public void UpdateEquipmentView()
    {
        equipItemCode = GameManager.Instance.GetEquipmentWeapon();

        _weaponDetail = WeaponManager.Instance.GetWeaponData(equipItemCode);
        Text_ItemName.text = _weaponDetail.GetWeaponName();
        Text_Ability.text = _weaponDetail.GetAttackPower().ToString();
        Image_Item.sprite = _weaponDetail.GetWeaponSprite();
    }

    /// <summary>
    /// 무기 상자를 생성합니다.
    /// </summary>
    void CreateWeaponBox()
    {
        List<string> codes = WeaponManager.Instance.GetWeaponCodeList();

        for (int i = 0; i < codes.Count; i++)
        {
            GameObject obj = Instantiate(WeaponBoxPrefabs, WeaponGridLayout);
            //WeaponManager.Instance.SetWeaponSprite(codes[i], Resources.Load<Sprite>("Sword/" + codes[i]));
            obj.GetComponent<WeaponBox>().SetWeaponData(codes[i]);
            obj.GetComponent<WeaponBox>().SetWeaponPanelController(this);
        }
    }
}
