using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyItemPanel : MonoBehaviour
{
    [SerializeField] GameObject coinPanel;
    [SerializeField] GameObject diamondPanel;

    private void OnDisable()
    {
        coinPanel.SetActive(false);
        diamondPanel.SetActive(false);
    }

    public void ShowCoinPanel()
    {
        coinPanel.SetActive(true);
    }

    public void ShowDiamondPanel()
    {
        diamondPanel.SetActive(true);
    }
}
