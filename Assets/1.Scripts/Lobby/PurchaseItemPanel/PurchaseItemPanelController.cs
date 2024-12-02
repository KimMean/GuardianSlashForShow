using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseItemPanelController : MonoBehaviour
{
    [SerializeField] WeaponItemListPanel weaponItemPanel;
    [SerializeField] NecklaceItemListPanel necklaceItemPanel;
    [SerializeField] RingItemListPanel ringItemPanel;
    [SerializeField] CurrencyItemPanel currencyItemPanel;


    public void SetWeaponItemPurchaseList(List<string> itemCodes)
    {
        weaponItemPanel.SetItemList(itemCodes);
    }
    public void SetNecklaceItemPurchaseList(List<string> itemCodes)
    {
        necklaceItemPanel.SetItemList(itemCodes);
    }
    public void SetRingItemPurchaseList(List<string> itemCodes)
    {
        ringItemPanel.SetItemList(itemCodes);
    }
    public void SetCoinItemPurchase()
    {
        currencyItemPanel.ShowCoinPanel();
    }
    public void SetDiamondItemPurchase()
    {
        currencyItemPanel.ShowDiamondPanel();
    }

    public void OnConfirmButtonClick()
    {
        gameObject.SetActive(false);
    }
}
