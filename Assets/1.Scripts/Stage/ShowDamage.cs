using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText;
    float duration = 1f; // �̵� �ð� (0.20��)
    float distance = 300f; // �̵� �Ÿ�
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float elapsedTime;

    private Action onDeactivate;

    private void Update()
    {
        if (!gameObject.activeSelf) return;
           
        if (elapsedTime < duration)
        {
            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            // ���� ��� (0 ~ 1)
            float t = elapsedTime / duration;

            // ��ġ ����
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
        }
        else
        {
            HideDamage();
        }
    }

    public void SetDamage(Vector2 position, long damage, Action onDeactivateCallback)
    {
        startPosition = position;
        transform.position = startPosition;
        endPosition = startPosition + new Vector3(0, distance, 0); // ���� 200 �̵�
        damageText.text = damage.ToString();
        onDeactivate = onDeactivateCallback;
        elapsedTime = 0;
        //GetComponent<Animator>().Play("Damage_Show");
    }

    public void HideDamage()
    {
        gameObject.SetActive(false);
        onDeactivate?.Invoke();
    }
}
