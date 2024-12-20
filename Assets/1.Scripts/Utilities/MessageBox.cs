using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] Text messageText;

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    public void SetText(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);

        //StopAllCoroutines();
        //StartCoroutine(HideMessage());
    }

    //IEnumerator HideMessage()
    //{
    //    yield return new WaitForSeconds(1);
    //    gameObject.SetActive(false);
    //}

    public void HideMessageBox()
    {
        gameObject.SetActive(false);
    }
}
