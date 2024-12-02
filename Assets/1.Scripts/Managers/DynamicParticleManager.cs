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

            // 머터리얼 복제 및 저장
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


        // 풀에서 파티클 가져오기
        GameObject particle = particlePool.Dequeue();
        particle.transform.position = position;
        particle.SetActive(true);

        // 개별 머터리얼 인스턴스 생성 및 텍스처 적용
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = texture; // 새로운 머터리얼 적용
        }

        // 파티클 재생 및 반환 처리
        StartCoroutine(ReturnToPool(particle));
    }

    private IEnumerator ReturnToPool(GameObject particle)
    {
        // 파티클 재생 대기 (파티클 수명 기반)
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constant);
        }

        // 풀에 다시 추가
        particle.SetActive(false);
        particlePool.Enqueue(particle);
    }
}
