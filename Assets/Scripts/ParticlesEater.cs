using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ParticlesEater : MonoBehaviour
{
    [FormerlySerializedAs("Mass")] public GameObject[] Particles;
    private PlayerActions _playerActions;

    void Start()
    {
        _playerActions = GetComponent<PlayerActions>();
        
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

    // TODO: fix this shit, it doesn't work
    public void CheckMass()
    {
        if (!_playerActions.OpenToConsume) return;
        
        Debug.Log("CheckMass");
        foreach (var t in Particles)
        {
            Transform m = t.transform;
            // can be optimized
            if (Vector2.Distance(transform.position, m.position) <= transform.localScale.x / 4)
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
