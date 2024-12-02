using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] Text MessageText;

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        MessageText.text = message;
        gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideMessage());
    }

    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
