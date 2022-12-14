using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ParticlesEater : MonoBehaviour
{
    [FormerlySerializedAs("Mass")] public GameObject[] Particles;

    void Start()
    {
        ParticlesSpawner.Instance.Players.Add(gameObject);
        UpdateMass();
        InvokeRepeating("CheckMass", 0, 0.1f);
    }

    void UpdateMass()
    {
        Particles = GameObject.FindGameObjectsWithTag("Particle"); 
    }


    public void AddMass(GameObject particleObject)
    {
        List<GameObject> particlesList = Particles.ToList();
        particlesList.Add(particleObject);
        Particles = particlesList.ToArray();

        ParticlesSpawner.Instance.AddMass(particleObject);
    }

    public void RemoveMass(GameObject particleObject)
    {
        List<GameObject> particlesList = Particles.ToList();
        particlesList.Remove(particleObject);
        Particles = particlesList.ToArray();

        ParticlesSpawner.Instance.RemoveMass(particleObject);
    }

    public void CheckMass()
    {
        foreach (var t in Particles)
        {
            Transform m = t.transform;
            // can be optimized
            if (Vector2.Distance(transform.position, m.position) <= transform.localScale.x / 3)
            {
                RemoveMass(m.gameObject);
                PlayerEat();
                Destroy(m.gameObject);
            }
        }
    }

    public void PlayerEat()
    {
        transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
    }
}
