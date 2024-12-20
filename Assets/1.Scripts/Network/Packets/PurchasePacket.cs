using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;
using static Packet;

public class PurchasePacket
{
    /*
     * 게임 내 재화로 구매합니다.
     * Command / Payment / Token / ProductID { / transactionID / receipt }
     */
    /// <summary>
    /// 아이템을 구매합니다.
    /// 패킷 크기 / 명령 / 지불 방식 / 토큰 / 제품 ID / {+ 식별코드 / 영수증 }
    /// </summary>
    /// <param name="payment"></param>
    /// <param name="token"></param>
    /// <param name="productID"></param>
    /// <param name="transactionID"></param>
    /// <param name="receipt"></param>
    /// <returns></returns>
    public static ArraySegment<byte> GetPurchaseRequest(Payment payment, string token, string productID, string transactionID = null, string receipt = null)
    {
        ArraySegment<byte> openSegment = SendBufferHelper.Open(1024);

        ushort count = 2;

        BitConverter.GetBytes((ushort)Command.Purchase).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        BitConverter.GetBytes((ushort)payment).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);

        byte[] userToken = Encoding.UTF8.GetBytes(token);
        ushort tokenSize = (ushort)userToken.Length;

        BitConverter.GetBytes(tokenSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        userToken.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += tokenSize;

        //Debug.Log($"User Token : {token}, tokenByte : {userToken}, tokenSize : {tokenSize}");
        byte[] product = Encoding.UTF8.GetBytes(productID);
        ushort productSize = (ushort)product.Length;

        BitConverter.GetBytes(productSize).CopyTo(openSegment.Array, openSegment.Offset + count);
        count += sizeof(ushort);
        product.CopyTo(openSegment.Array, openSegment.Offset + count);
        count += productSize;
        //Debug.Log($"productID : {productID}, productByte : {product}, productSize : {productSize}");

        if (payment == Payment.Google)
        {
            byte[] transaction = Encoding.UTF8.GetBytes(transactionID);
            ushort transactionSize = (ushort)transactionID.Length;

            BitConverter.GetBytes(transactionSize).CopyTo(openSegment.Array, openSegment.Offset + count);
            count += sizeof(ushort);
            transaction.CopyTo(openSegment.Array, openSegment.Offset + count);
            count += transactionSize;

            byte[] _receipt = Encoding.UTF8.GetBytes(receipt);
            ushort receiptSize = (ushort)receipt.Length;

            BitConverter.GetBytes(receiptSize).CopyTo(openSegment.Array, openSegment.Offset + count);
            count += sizeof(ushort);
            _receipt.CopyTo(openSegment.Array, openSegment.Offset + count);
            count += receiptSize;
        }

        BitConverter.GetBytes(count).CopyTo(openSegment.Array, openSegment.Offset);

        return SendBufferHelper.Close(count);
    }

    /// <summary>
    /// 구매 결과를 받습니다.
    /// 반환 결과 / 제품 종류
    /// 재화 구매시 + 코인 , 다이아
    /// 무기, 목걸이, 반지 구매시 + 아이템 코드 리스트와 고유 정보
    /// </summary>
    public bool ReceivePurchaseResponse(ArraySegment<byte> buffer)
    {
        Debug.Log("ReceivePurchaseResponse 받는 중");
        
        int parsingCount = 0;

        ushort result = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        parsingCount += sizeof(ushort);

        //if((ResultCommand)result == ResultCommand.Success)
        //{
        //    Debug.Log("Purchase Success");
        //}
        if((ResultCommand)result == ResultCommand.Failed)
        {
            Debug.Log("Purchase Failed");
            return false;
        }
        Debug.Log("Purchase Success");

        Products product = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        switch (product)
        {
            case Products.Coin:
            case Products.Diamond:
                {

                    // 보유 코인 업데이트
                    Products productCoin = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    int coin = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(int);

                    // 보유 다이아 업데이트
                    Products productDiamond = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    int dia = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(int);

                    GameManager.Instance.SetCoin(coin);
                    GameManager.Instance.SetDiamond(dia);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        if(product == Products.Coin)
                            StoreController.Instance.PurchaseCoin();
                        else
                            StoreController.Instance.PurchaseDiamond();
                    });
                    break;
                }
            case Products.Weapon:
                {
                    Debug.Log($"Products.Weapon : {Products.Weapon}");
                    // 무기 개수 증가
                    ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    string itemList = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, size);
                    parsingCount += size;

                    Products productCoin = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    int coin = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(int);

                    Products productDiamond = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    int dia = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(int);

                    //Debug.Log($"WeaponList : {itemList}");
                    // Split 메서드를 사용하여 문자열을 나누고, List<string>에 저장
                    List<string> itemCodesList = new List<string>(itemList.Split(','));
                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        StoreController.Instance.PurchaseWeaponList(itemCodesList);
                    });
                    WeaponManager.Instance.AddItems(itemCodesList);
                    GameManager.Instance.SetCoin(coin);
                    GameManager.Instance.SetDiamond(dia);
                    Debug.Log($"Coin : {coin}, Diamond : {dia}");

                    break;
                }
            case Products.Necklace:
                {
                    // 목걸이 개수 증가
                    ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    string itemList = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, size);
                    parsingCount += size;

                    Products productCoin = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    int coin = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(int);

                    Products productDiamond = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    int dia = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(int);

                    //Debug.Log($"NecklaceList : {itemList}");
                    // Split 메서드를 사용하여 문자열을 나누고, List<string>에 저장
                    List<string> itemCodesList = new List<string>(itemList.Split(','));


                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        StoreController.Instance.PurchaseNecklaceList(itemCodesList);
                    });
                    NecklaceManager.Instance.AddItems(itemCodesList);
                    GameManager.Instance.SetCoin(coin);
                    GameManager.Instance.SetDiamond(dia);
                    Debug.Log($"Coin : {coin}, Diamond : {dia}");

                    break;
                }
            case Products.Ring:
                {
                    // 반지 개수 증가
                    ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    string itemList = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, size);
                    parsingCount += size;

                    Products productCoin = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    int coin = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(int);

                    Products productDiamond = (Products)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(ushort);
                    int dia = BitConverter.ToInt32(buffer.Array, buffer.Offset + parsingCount);
                    parsingCount += sizeof(int);

                    //Debug.Log($"RingList : {itemList}");
                    // Split 메서드를 사용하여 문자열을 나누고, List<string>에 저장
                    List<string> itemCodesList = new List<string>(itemList.Split(','));

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        StoreController.Instance.PurchaseRingList(itemCodesList);
                    });
                    RingManager.Instance.AddItems(itemCodesList);
                    GameManager.Instance.SetCoin(coin);
                    GameManager.Instance.SetDiamond(dia);
                    Debug.Log($"Coin : {coin}, Diamond : {dia}");

                    break;
                }
            default:
                break;
        }

        Payment payment = (Payment)BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
        parsingCount += sizeof(ushort);

        if(payment == Payment.Google)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + parsingCount);
            parsingCount += sizeof(ushort);
            string transactionID = Encoding.UTF8.GetString(buffer.Array, buffer.Offset + parsingCount, size);
            parsingCount += size;
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                IAPManager.Instance.ExcuteProcessPendingPurchase(transactionID);
            });
        }

        return true;
    }
}
