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

    private bool IsInitialized = false;

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

        IsInitialized = true;
    }
    
    // �ʱ�ȭ ����
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP Initialize Failed : {error}");
    }

    // �ʱ�ȭ ����
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Initialize Failed : {error}, Message : {message}");
    }

    // ���� ����
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"IAP Purchase Failed, Purchase Failure Reason : {failureReason},\n Product ID : {product.definition.id}");
    }

    // ���� ó��
    //($"��ǰ ID: {product.definition.id}, ����: {product.metadata.localizedPriceString}");
    //($"��ǰ �̸� : {product.metadata.localizedTitle}");
    //($"��ǰ ���� : {product.metadata.localizedDescription}");
    //($"��ǰ ����? : {product.definition.payout}");
    //($"��ǰ ID? : {product.definition.storeSpecificId}, {product.definition.payout}");
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log($"IAP Process Purchase, Purchase Processing Result : {purchaseEvent.purchasedProduct},\n Product ID : {purchaseEvent.purchasedProduct.definition.id}");
        Debug.Log($"TransactionID : {purchaseEvent.purchasedProduct.transactionID}");
        Debug.Log($"Receipt : {purchaseEvent.purchasedProduct.receipt}");

        Product product = purchaseEvent.purchasedProduct;
        string productID = product.definition.id;
        string transactionID = product.transactionID;
        string receipt = product.receipt;

        // �������� �����ϰ� ������ ���ޱ��� �Ϸ�Ȱ�� �������� ������ �Ϸ�ó���մϴ�.
        // ���� ����� ���������� �޾ƾ� ����˴ϴ�.
        // ������ ���� ���� ��� ������ �������·� �����ֽ��ϴ�.
        Debug.Log("pendingRequests[transactionID] = () => ProcessPendingPurchase(product);");
        pendingRequests[transactionID] = () => ProcessPendingPurchase(product);
        Debug.Log("NetworkManager.Instance.ItemPurchase(Packet.Payment.Google, productID, transactionID, receipt);");
        NetworkManager.Instance.ItemPurchase(Packet.Payment.Google, productID, transactionID, receipt);
        // Ʈ����� ID�� �������� ������ �ϴ°� ���� �� ������
        // �ڷ�ƾ �Ǵ� �ݹ�
        // ���� ���
        // ������ ������ �α� �ۼ�
        // �������� ���� ������ ����(����)
        // ������ ���� ���н� ������..?
        // PurchaseProcessingResult.Pending�� ���

        return PurchaseProcessingResult.Pending;
    }

    // �������� �����ϰ� ������ ���ޱ��� �Ϸ�Ȱ�� �������� ������ �Ϸ�ó���մϴ�.
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

    public void Purchase(string productId)
    {
        if (!IsInitialized)
        {
            return;
        }

        Product product = controller.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"���� �õ� - {product.definition.id}");
            controller.InitiatePurchase(product);
        }
        else
        {
            Debug.Log($"���� �õ� �Ұ� - {productId}");
        }
    }

}
