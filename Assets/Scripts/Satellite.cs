using interfaces;
using UnityEngine;

static class Constants
{
    public static readonly float G = 9.81f;
}

public class Satellite : MonoBehaviour, ISatellite
{
    public IAttractor CurrentAttractor { get => currentAttractor; set => currentAttractor = (Attractor)value; }//shit
    public Attractor currentAttractor;

    public bool IsOnOrbit;
    
    private Transform _currentAttractorTransform;
    private Rigidbody2D _currentAttractorRigidbody2D;

    private Rigidbody2D _rigidbody2D;
    
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (CurrentAttractor != null)
        {
            UpdateOrbitalMovement();
        }
    }

    public void Attach(IAttractor attractor)
    {
        CurrentAttractor = attractor;
        
        GameObject attractorGameObject = CurrentAttractor.GetGameObject();
        
        _currentAttractorTransform = attractorGameObject.GetComponent<Transform>();
        _currentAttractorRigidbody2D = attractorGameObject.GetComponent<Rigidbody2D>();
    }   

    public void Detach()
    {
        CurrentAttractor = null;

        _currentAttractorTransform = null;
        _currentAttractorRigidbody2D = null;
    }

    public void UpdateOrbitalMovement()
    {
        Vector3 attractorPosition = _currentAttractorTransform.position;
        Vector3 satellitePosition = transform.position;
        float attractorMass = _currentAttractorRigidbody2D.mass;
        float satelliteMass = _rigidbody2D.mass;
        
        float speedScale = currentAttractor.SpeedScale;
        float orbitRadius = currentAttractor.OrbitRadius;

        Vector2 finalForce = Vector2.zero;
        
        if (!IsOnOrbit) // just a common gravitation
        {
            float r = Vector3.Distance(attractorPosition, satellitePosition);

            // 1. Do we need such a fair calculations of the force? Because we should scale it after that in any way
            // 2. TODO: Add delta time to the calculations
            float totalForce = -(Constants.G * attractorMass * satelliteMass) / (r * r);
            Vector2 direction = (satellitePosition - attractorPosition);
            
            finalForce = direction.normalized * totalForce;
        }
        else  // orbit movement
        {
            // value to calculate the proportion between 'rotation' and 'attraction' force.
            // - 0f to 1f   - when satellite closer to the attractor than $orbitRadius,
            // - 1f         - when satellite exact on orbit,
            // - 1f to inf  - when satellite is farther than $orbitRadius.
            float stabilizer = Vector3.Distance(attractorPosition, satellitePosition) / orbitRadius;

            Vector2 directionClockwise = transform.up * (1f / Mathf.Sqrt(stabilizer));
            Vector2 directionToAttractor =transform.right;
                

            finalForce = (directionToAttractor + directionClockwise) * (speedScale * Time.deltaTime);
            
            // // Debug diff between actual and desired orbits. Could affect fps
            // Debug.Log(stabilizer);
            //
            // // Debug existing and additional forces as vectors. Could affect fps
            // DebugExtensions.DrawArrow(transform.position, finalForce, Color.magenta);
            // DebugExtensions.DrawArrow(transform.position, _rigidbody2D.velocity, Color.red);
        }
        
        // rotate to "look" on the attractor
        transform.right = attractorPosition - satellitePosition;

        // move
        _rigidbody2D.AddForce(finalForce);
    }
}
