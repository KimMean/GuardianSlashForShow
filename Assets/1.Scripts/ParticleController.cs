using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private float duration;


    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        duration = particleSystem.main.duration + particleSystem.main.startLifetime.constant;
    }

    private void OnEnable()
    {
        StartCoroutine(ParticleTurnOffDelay());
    }

    /// <summary>
    /// ��ƼŬ�� ��Ȱ�� ���·� ����ϴ�.
    /// </summary>
    /// <returns>Delay by duration</returns>
    IEnumerator ParticleTurnOffDelay()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }

}
