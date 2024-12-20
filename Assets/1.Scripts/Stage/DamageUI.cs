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
    /// <summary>
    /// ������Ʈ Ǯ�� �ʱ�ȭ �մϴ�.
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject damageUI = Instantiate(damagePrefab, transform);
            damageUI.SetActive(false);
            damagePool.Enqueue(damageUI);
        }
    }

    /// <summary>
    /// ������Ʈ Ǯ���� UI�� �����ɴϴ�.
    /// </summary>
    /// <returns></returns>
    private GameObject GetFromPool()
    {
        if (damagePool.Count > 0)
        {
            GameObject ui = damagePool.Dequeue();
            return ui;
        }
        else
        {
            // Ǯ�� ����� ��� ���� ���� (�ɼ�)
            GameObject damageUI = Instantiate(damagePrefab, transform);
            return damageUI;
        }
    }

    /// <summary>
    /// ������Ʈ�� �ٽ� Ǯ�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="damageUI"></param>
    private void ReturnToPool(GameObject damageUI)
    {
        //Debug.Log("Return To Pool");
        damageUI.SetActive(false);
        damagePool.Enqueue(damageUI);
    }

    /// <summary>
    /// ������ UI�� ǥ���մϴ�.
    /// </summary>
    /// <param name="position">�������� ǥ�õ� ��ġ</param>
    /// <param name="damage">���ط�</param>
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
