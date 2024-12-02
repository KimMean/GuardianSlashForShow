using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicParticleManager : MonoBehaviour
{
    [SerializeField] GameObject particlePrefab;
    [SerializeField] Material materialPrefab;
    [SerializeField] private int poolSize = 10;

    Queue<GameObject> particlePool;
    ParticleSystemRenderer particleRenderers;

    private void Awake()
    {
        particlePool = new Queue<GameObject>();

        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity);
            particle.SetActive(false);
            particlePool.Enqueue(particle);

            // ���͸��� ���� �� ����
            Material materialInstance = new Material(materialPrefab);
            particle.GetComponent<ParticleSystemRenderer>().material = materialInstance;

        }
    }

    private void OnDestroy()
    {
        //while (particlePool.Count > 0)
        //{
        //    GameObject particle = particlePool.Dequeue();
        //    Destroy(particle.GetComponent<ParticleSystemRenderer>().material);
        //    Destroy(particle);
        //}
        //particlePool.Clear();
    }

    public void PlayParticle(Vector2 position, Texture texture)
    {
        if (particlePool.Count == 0) return;


        // Ǯ���� ��ƼŬ ��������
        GameObject particle = particlePool.Dequeue();
        particle.transform.position = position;
        particle.SetActive(true);

        // ���� ���͸��� �ν��Ͻ� ���� �� �ؽ�ó ����
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = texture; // ���ο� ���͸��� ����
        }

        // ��ƼŬ ��� �� ��ȯ ó��
        StartCoroutine(ReturnToPool(particle));
    }

    private IEnumerator ReturnToPool(GameObject particle)
    {
        // ��ƼŬ ��� ��� (��ƼŬ ���� ���)
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constant);
        }

        // Ǯ�� �ٽ� �߰�
        particle.SetActive(false);
        particlePool.Enqueue(particle);
    }
}
