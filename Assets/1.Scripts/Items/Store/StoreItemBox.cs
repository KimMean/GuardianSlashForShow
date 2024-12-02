using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static InAPP_ProductManager;
using static Packet;

public class StoreItemBox : MonoBehaviour
{
    private enum CurrencyType
    {
        Coin,
        Diamond,
        KRW,
    }

    [SerializeField] Text titleText;
    [SerializeField] Text priceText;
    [SerializeField] MessageBox messageBox;


    [Header("ProductInfo")]
    [SerializeField] string productID;
    [SerializeField, ReadOnly] string productName;
    [SerializeField, ReadOnly] private Payment payment;
    [SerializeField, ReadOnly] private CurrencyType currencyType;
    [SerializeField, ReadOnly] private int price;

    private void Awake()
    {
        InAPP_Product product = InAPP_ProductManager.Instance.GetInAPP_ProductData(productID);
        productName = product.productName;
        price = product.price;

        titleText.text = productName;
        priceText.text = price.ToString();

        if (product.currencyType == Products.Coin)
        {
            payment = Payment.Local;
            currencyType = CurrencyType.Coin;
        }
        else if(product.currencyType == Products.Diamond)
        {
            payment= Payment.Local;
            currencyType = CurrencyType.Diamond;
        }
        else
        {
            payment = Payment.Google;
            currencyType = CurrencyType.KRW;
            priceText.text = "KRW " + price.ToString();
        }

    }

    public void OnPurchaseButtonClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        if (currencyType == CurrencyType.Coin)
        {
            if(GameManager.Instance.GetCoin() < price)
            {
                messageBox.ShowMessage("보유한 재화가 부족합니다.");
                Debug.Log("보유한 재화가 부족합니다.");
                return;
            }
        }
        else if(currencyType == CurrencyType.Diamond)
        {
            if (GameManager.Instance.GetDiamond() < price)
            {
                messageBox.ShowMessage("보유한 재화가 부족합니다.");
                Debug.Log("보유한 재화가 부족합니다.");
                return;
            }
        }
        else
        {
            Debug.Log("구글 인앱 결제 프로세스");
            IAPManager.Instance.Purchase(productID);
            return;
        }

        NetworkManager.Instance.ItemPurchase(payment, productID);
    }
}
