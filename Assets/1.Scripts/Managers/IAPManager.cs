using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IAPManager instance;
    public static IAPManager Instance
    {
        get
        {
            if(instance == null)
                instance = FindObjectOfType<IAPManager>();

            return instance;
        }
    }

    private IStoreController controller;
    private IExtensionProvider extensions;

    private bool isInitialized = false;

    private Dictionary<string, Action> pendingRequests;


    private void Awake()
    {
        if(instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);

        InitUnityIAP();
        pendingRequests = new Dictionary<string, Action>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// UnityIAP를 초기화합니다.
    /// </summary>
    private void InitUnityIAP()
    {
        Debug.Log("InitIAP");
        StandardPurchasingModule module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
        builder.AddProduct("diamond250", ProductType.Consumable);
        builder.AddProduct("diamond550", ProductType.Consumable);
        builder.AddProduct("diamond1200", ProductType.Consumable);
        builder.AddProduct("diamond2800", ProductType.Consumable);
        builder.AddProduct("diamond6000", ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP Initialized");
        this.controller = controller;
        this.extensions = extensions;

        isInitialized = true;
    }
    
    /// <summary>
    /// 초기화 실패
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP Initialize Failed : {error}");
    }

    /// <summary>
    /// 초기화 실패
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Initialize Failed : {error}, Message : {message}");
    }

    /// <summary>
    /// 구매 실패
    /// </summary>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"IAP Purchase Failed, Purchase Failure Reason : {failureReason},\n Product ID : {product.definition.id}");
    }

    /// <summary>
    /// 구매 처리
    /// </summary>
    /// <param name="purchaseEvent"></param>
    /// <returns></returns>
    /// <remarks>
    /// ($"상품 ID: {product.definition.id}, 가격: {product.metadata.localizedPriceString}");
    /// ($"상품 이름 : {product.metadata.localizedTitle}");
    /// ($"상품 설명 : {product.metadata.localizedDescription}");
    /// ($"상품 가격? : {product.definition.payout}");
    /// ($"상품 ID? : {product.definition.storeSpecificId}, {product.definition.payout}");
    /// </remarks>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log($"IAP Process Purchase, Purchase Processing Result : {purchaseEvent.purchasedProduct},\n Product ID : {purchaseEvent.purchasedProduct.definition.id}");
        Debug.Log($"TransactionID : {purchaseEvent.purchasedProduct.transactionID}");
        Debug.Log($"Receipt : {purchaseEvent.purchasedProduct.receipt}");

        Product product = purchaseEvent.purchasedProduct;
        string productID = product.definition.id;
        string transactionID = product.transactionID;
        string receipt = product.receipt;

        // 영수증을 저장하고 아이템 지급까지 완료된경우 보류중인 결제를 완료처리합니다.
        // 소켓 통신을 정상적으로 받아야 실행됩니다.
        // 응답을 받지 못한 경우 결제는 보류상태로 남아있습니다.
        Debug.Log("pendingRequests[transactionID] = () => ProcessPendingPurchase(product);");
        pendingRequests[transactionID] = () => ProcessPendingPurchase(product);
        Debug.Log("NetworkManager.Instance.ItemPurchase(Packet.Payment.Google, productID, transactionID, receipt);");
        NetworkManager.Instance.ItemPurchase(Packet.Payment.Google, productID, transactionID, receipt);
       

        return PurchaseProcessingResult.Pending;
    }

    /// <summary>
    /// 영수증을 저장하고 아이템 지급까지 완료된경우 보류중인 결제를 완료처리합니다.
    /// </summary>
    /// <param name="purchasedProduct"></param>
    public void ProcessPendingPurchase(Product purchasedProduct)
    {
        Debug.Log($"ProcessPendingPurchase, ProductID : {purchasedProduct.definition.id}");
        if (purchasedProduct == null)
        {
            Debug.LogError("purchasedProduct is null");
            return;
        }
        if (controller == null)
        {
            Debug.LogError("controller is null");
            return;
        }
        controller.ConfirmPendingPurchase(purchasedProduct);
    }
    
    /// <summary>
    /// 영수증이 정상적으로 저장된 경우 응답을 받습니다.
    /// </summary>
    /// <param name="transactionID"></param>
    public void ExcuteProcessPendingPurchase(string transactionID)
    {
        Debug.Log($"ExcuteProcessPendingPurchase TransactionID : {transactionID}");
        if (pendingRequests.ContainsKey(transactionID))
        {
            Debug.Log($"Contain TransactionID : {transactionID}");
            pendingRequests[transactionID].Invoke();
            pendingRequests.Remove(transactionID);
        }
    }

    /// <summary>
    /// IAP 프로세스를 실행합니다.
    /// </summary>
    public void Purchase(string productId)
    {
        if (!isInitialized)
        {
            return;
        }

        Product product = controller.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"구매 시도 - {product.definition.id}");
            controller.InitiatePurchase(product);
        }
        else
        {
            Debug.Log($"구매 시도 불가 - {productId}");
        }
    }

}
