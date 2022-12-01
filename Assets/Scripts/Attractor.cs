using System.Collections;
using System.Collections.Generic;
using System.Linq;
using interfaces;
using models;
using UnityEngine;

public class Attractor : MonoBehaviour, IAttractor
{
    public LayerMask AttractionLayer;
    [SerializeField]
    private float effectionRadius = 10;
    
    public IEnumerable<ISatellite> Satellites { get; set; }

    void Start()
    {
        SetAttractedObjects();
    }

    void Update()
    {
        SetAttractedObjects();
    }

    public Ellipse GetOrbit()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetGameObject() => this.gameObject;
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, effectionRadius);
    }
    
    void SetAttractedObjects()
    {
        var satellites = Physics2D.OverlapCircleAll(transform.position, effectionRadius, AttractionLayer);
            // .Select(x => x.GetComponent<Satellite>());// can be optimized

        foreach (var satellite in satellites)
        {
            Debug.Log($"Attaching satellite: {satellite}");
            satellite.GetComponent<Satellite>().Attach(this);
            satellite.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
