using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecklaceItemListPanel : MonoBehaviour
{
    [SerializeField] GameObject[] itemBoxes;

    private void OnDisable()
    {
        foreach (GameObject itemBox in itemBoxes)
            itemBox.SetActive(false);
    }

    public void SetItemList(List<string> itemCodes)
    {
        for (int i = 0; i < itemCodes.Count; i++)
        {
            itemBoxes[i].GetComponent<NecklaceItemBox>().SetNecklace(itemCodes[i]);
            itemBoxes[i].SetActive(true);
        }
    }
}