using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    [SerializeField] int poolSize = 10;
    [SerializeField] GameObject damagePrefab;

    Queue<GameObject> damagePool = new Queue<GameObject>();

    private void Awake()
    {
        InitializePool();
    }
    // 오브젝트 풀 초기화
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject damageUI = Instantiate(damagePrefab, transform);
            damageUI.SetActive(false);
            damagePool.Enqueue(damageUI);
        }
    }

    // 오브젝트 풀에서 UI 가져오기
    private GameObject GetFromPool()
    {
        if (damagePool.Count > 0)
        {
            GameObject ui = damagePool.Dequeue();
            return ui;
        }
        else
        {
            // 풀이 비었을 경우 새로 생성 (옵션)
            GameObject damageUI = Instantiate(damagePrefab, transform);
            return damageUI;
        }
    }

    // 오브젝트를 다시 풀로 반환
    private void ReturnToPool(GameObject damageUI)
    {
        //Debug.Log("Return To Pool");
        damageUI.SetActive(false);
        damagePool.Enqueue(damageUI);
    }

    // 데미지 UI 표시
    public void ShowDamageUI(Vector3 position, long damage)
    {
        GameObject damageUI = GetFromPool();

        ShowDamage damageUIScript = damageUI.GetComponent<ShowDamage>();
        if (damageUIScript != null)
        {
            damageUIScript.SetDamage(position, damage, () => ReturnToPool(damageUI));
            damageUI.SetActive(true);
        }
    }
}
