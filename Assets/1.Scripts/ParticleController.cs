using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private float duration;


    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        duration = _particleSystem.main.duration + _particleSystem.main.startLifetime.constant;
    }

    private void OnEnable()
    {
        StartCoroutine(ParticleTurnOffDelay());
    }

    IEnumerator ParticleTurnOffDelay()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
