using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControll : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private ParticleSystem[] particles;

    private void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        foreach (ParticleSystem particle in particles)
        {
            ParticleSystem.MainModule main = particle.main;
            main.simulationSpeed = speed;
        }
    }

    public float GetParticleSpeed()
    {
        return this.speed;
    }
}
