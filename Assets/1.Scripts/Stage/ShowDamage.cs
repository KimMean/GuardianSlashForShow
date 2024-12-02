using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText;
    float duration = 1f; // 이동 시간 (0.20초)
    float distance = 300f; // 이동 거리
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float elapsedTime;

    private Action onDeactivate;

    private void Update()
    {
        if (!gameObject.activeSelf) return;
           
        if (elapsedTime < duration)
        {
            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            // 비율 계산 (0 ~ 1)
            float t = elapsedTime / duration;

            // 위치 보간
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
        endPosition = startPosition + new Vector3(0, distance, 0); // 위로 200 이동
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
