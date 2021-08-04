using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ParticleSystem particle;

    private void OnCollisionEnter(Collision collision)
    {
        particle.Stop();        
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle has collided");
        particle.Stop();
    }
}
