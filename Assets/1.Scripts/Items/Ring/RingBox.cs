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
    /// <summary>
    /// 새로운 아이템을 얻은 경우 잠금을 해제합니다.
    /// </summary>
    public void OnUnlocked()
    {
        if (Locked.activeSelf)
            Locked.SetActive(false);
    }
    /// <summary>
    /// 아이템 박스 클릭
    /// </summary>
    public void OnRingBoxClicked()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (_RingDetail.GetIsPossess())
        {
            _RingPanelController.ShowRingDetailPopup(ItemCode);
        }
    }
}
