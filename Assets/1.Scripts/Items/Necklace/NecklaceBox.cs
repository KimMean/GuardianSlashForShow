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
    /// <summary>
    /// 새로운 목걸이를 얻은 경우 잠금을 해제합니다.
    /// </summary>
    public void OnUnlocked()
    {
        if (Locked.activeSelf)
            Locked.SetActive(false);
    }

    public void SetNecklacePanelController(NecklacePanelController necklacePanelController)
    {
        _NecklacePanelController = necklacePanelController;
    }
    /// <summary>
    /// 아이템 박스를 클릭한 경우 세부 정보를 보여줍니다.
    /// </summary>
    public void OnNecklaceBoxClicked()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);

        if (_NecklaceDetail.GetIsPossess())
        {
            _NecklacePanelController.ShowNecklaceDetailPopup(ItemCode);
        }
    }

}
