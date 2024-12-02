using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Packet;

public class InAPP_ProductManager
{
    private static InAPP_ProductManager instance;
    public static InAPP_ProductManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InAPP_ProductManager();
            }
            return instance;
        }
    }

    public struct InAPP_Product
    {
        public readonly string productID;
        public readonly string productName;
        public readonly Products currencyType;
        public readonly int price;

        public InAPP_Product(string id, string name, Products currencyType, int price)
        {
            productID = id;
            productName = name;
            this.currencyType = currencyType;
            this.price = price;

        }
    }

    Dictionary<string, InAPP_Product> InAPP_Products = new Dictionary<string, InAPP_Product>();

    public void SetInAPP_ProductData(string id, string name, Products currencyType, int price)
    {
        InAPP_Products[id] = new InAPP_Product(id, name, currencyType, price);
        //Debug.Log($"SetInAPP_ProductData ID : {id}, Name : {name}, CurrencyType : {currencyType}, Price : {price}");
    }
    
    public InAPP_Product GetInAPP_ProductData(string id)
    {
        return InAPP_Products[id];
    }
}
