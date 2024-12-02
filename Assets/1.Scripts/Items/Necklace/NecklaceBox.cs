using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NecklaceBox : MonoBehaviour
{
    NecklacePanelController _NecklacePanelController;
    public string ItemCode;

    NecklaceDetail _NecklaceDetail;
    [SerializeField] Image Image_Item;
    [SerializeField] GameObject Locked;

    private void OnDestroy()
    {
        if(_NecklaceDetail != null)
            _NecklaceDetail.OnNewItemAcquired -= OnUnlocked;
    }

    public void SetNecklaceData(string code)
    {
        ItemCode = code;
        _NecklaceDetail = NecklaceManager.Instance.GetNecklaceData(ItemCode);
        _NecklaceDetail.OnNewItemAcquired += OnUnlocked;

        Image_Item.sprite = _NecklaceDetail.GetNecklaceSprite();
        Locked.SetActive(!_NecklaceDetail.GetIsPossess());
    }

    public void OnUnlocked()
    {
        if (Locked.activeSelf)
            Locked.SetActive(false);
    }

    public void SetNecklacePanelController(NecklacePanelController necklacePanelController)
    {
        _NecklacePanelController = necklacePanelController;
    }

    public void OnNecklaceBoxClicked()
    {
        if (_NecklaceDetail.GetIsPossess())
        {
            _NecklacePanelController.ShowNecklaceDetailPopup(ItemCode);
        }
    }

}
