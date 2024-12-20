using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] GameObject ParticlePrefab;

    [SerializeField] private int MaxParticles = 10;

    private List<GameObject> particles;

    private void Awake()
    {
        particles = new List<GameObject>();
    }

    private void OnEnable()
    {
        for(int i = 0; i < MaxParticles; i++)
        {
            GameObject particle = Instantiate(ParticlePrefab, Vector3.zero, Quaternion.identity);
            //particle.SetActive(false);
            particles.Add(particle);
        }
    }

    private void OnDisable()
    {
        foreach(GameObject particle in particles)
        {
            Destroy(particle);
        }
        particles.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ParticleActivation(Vector2 position)
    {
        foreach (GameObject particle in particles)
        {
            if (particle.activeSelf) continue;

            particle.transform.position = position;
            particle.SetActive(true);
            break;

        }
    }
}
