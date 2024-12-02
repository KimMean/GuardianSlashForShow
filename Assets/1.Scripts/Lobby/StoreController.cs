using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using static Packet;

public class StoreController : MonoBehaviour
{
    private static StoreController instance;
    public static StoreController Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] GameObject purchaseItemPanel;

    [SerializeField] RectTransform scrollViewContent;

    [SerializeField] GameObject probabilityPanel;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        scrollViewContent.anchoredPosition = new Vector2(0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PurchaseWeaponList(List<string> itemCodes)
    {
        purchaseItemPanel.SetActive(true);
        purchaseItemPanel.GetComponent<PurchaseItemPanelController>().SetWeaponItemPurchaseList(itemCodes);
    }

    public void PurchaseNecklaceList(List<string> itemCodes)
    {
        purchaseItemPanel.SetActive(true);
        purchaseItemPanel.GetComponent<PurchaseItemPanelController>().SetNecklaceItemPurchaseList(itemCodes);
    }

    public void PurchaseRingList(List<string> itemCodes)
    {
        purchaseItemPanel.SetActive(true);
        purchaseItemPanel.GetComponent<PurchaseItemPanelController>().SetRingItemPurchaseList(itemCodes);
    }

    public void PurchaseDiamond()
    {
        purchaseItemPanel.SetActive(true);
        purchaseItemPanel.GetComponent<PurchaseItemPanelController>().SetDiamondItemPurchase();
    }

    public void PurchaseCoin()
    {
        purchaseItemPanel.SetActive(true);
        purchaseItemPanel.GetComponent<PurchaseItemPanelController>().SetCoinItemPurchase();
    }

    public void ShowWeaponProbability()
    {
        probabilityPanel.SetActive(true);
        probabilityPanel.GetComponent<ItemProbabilityTable>().ShowProbabilityTable(Products.Weapon);
    }
    public void ShowNecklaceProbability()
    {
        probabilityPanel.SetActive(true);
        probabilityPanel.GetComponent<ItemProbabilityTable>().ShowProbabilityTable(Products.Necklace);
    }
    public void ShowRingProbability()
    {
        probabilityPanel.SetActive(true);
        probabilityPanel.GetComponent<ItemProbabilityTable>().ShowProbabilityTable(Products.Ring);
    }
}
