using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using interfaces;
using models;
using UnityEngine;

public class Attractor : MonoBehaviour, IAttractor
{
    public LayerMask AttractionLayer;    
    public CircleCollider2D _orbitCollider;
    public CircleCollider2D _innerCollider;
    public Rigidbody2D _rigidbody2D;

    public float DBG_RotationSpeedScale = 0.01f;
    public float DBG_RotationSpeedLimit = 3.5f;
    public float DBG_PushOutForceLimit = 2f;
    public float DBG_OrbitDistanceDiff = 0f;

    [SerializeField]
    private float effectionRadius = 10;



    private const float MAX_VELOCITY_MAGNITUDE = 1.3f;
    private const float MIN_VELOCITY_MAGNITUDE = 0f;

    // middle radius between _innerCollider and _orbitCollider plus DBG_OrbitDistanceDiff 
    public float OrbitDistance => ((_orbitCollider.radius + _innerCollider.radius) * transform.localScale.x) / 2f + DBG_OrbitDistanceDiff;
                                  // * (1 - (_rigidbody2D.velocity.magnitude / MAX_VELOCITY_MAGNITUDE) / 1.5f); // think about this part, most likely remove
    
    public IEnumerable<ISatellite> Satellites { get; set; }



    void Start()
    {
        SetAttractedObjects();
        
        // var colliders = GetComponents<CircleCollider2D>();
        // _orbitCollider = colliders[0].radius > colliders[1].radius ? colliders[0] : colliders[1]; // yeah, very bad
        Debug.Log("orbit radius - " + OrbitDistance);
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
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, OrbitDistance);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        Satellite satellite = other.GetComponent<Satellite>();

        if (satellite)
        {
            Debug.Log("Entered orbit");
            satellite.IsOnOrbit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Satellite satellite = other.GetComponent<Satellite>();

        if (satellite)
        {
            Debug.Log("Exited orbit");
            satellite.IsOnOrbit = false;
        }
    }
}
