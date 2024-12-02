using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingBox : MonoBehaviour
{
    public string ItemCode;
    RingPanelController _RingPanelController;

    RingDetail _RingDetail;
    [SerializeField] Image Image_Item;
    [SerializeField] GameObject Locked;
    private void OnDestroy()
    {
        if (_RingDetail != null)
            _RingDetail.OnNewItemAcquired -= OnUnlocked;
    }

    public void SetRingPanelController(RingPanelController ringPanelController)
    {
        _RingPanelController = ringPanelController;
    }

    public void SetRingData(string code)
    {
        ItemCode = code;
        _RingDetail = RingManager.Instance.GetRingData(ItemCode);
        _RingDetail.OnNewItemAcquired += OnUnlocked;

        Image_Item.sprite = _RingDetail.GetRingSprite();
        Locked.SetActive(!_RingDetail.GetIsPossess());
    }

    public void OnUnlocked()
    {
        if (Locked.activeSelf)
            Locked.SetActive(false);
    }

    public void OnRingBoxClicked()
    {
        if (_RingDetail.GetIsPossess())
        {
            _RingPanelController.ShowRingDetailPopup(ItemCode);
        }
    }
}
