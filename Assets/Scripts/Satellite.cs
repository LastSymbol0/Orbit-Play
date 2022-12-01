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

        // SetInitialVelocity();
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

        bool addExtraRotation = _rigidbody2D.velocity.magnitude < 3f;

        if (addExtraRotation)
        {
            Debug.Log("Rotating");
        }
        
        float r = Vector3.Distance(attractorPosition, satellitePosition);
        float totalForce = -(Constants.G * attractorMass * satelliteMass) / (r * r);
        Vector2 direction = (satellitePosition - attractorPosition);
        Vector2 additionalRotationForce = addExtraRotation ? transform.up : Vector2.zero;
        Vector2 force = (direction + additionalRotationForce).normalized * totalForce;
        
        // move
        _rigidbody2D.AddForce(force);
        
        //rotate to "look" on the attractor
        transform.right = attractorPosition - satellitePosition;
    }

    private void SetInitialVelocity()
    {
        Vector3 satellitePosition = transform.position;
        float attractorMass = _currentAttractorRigidbody2D.mass;
        
        float initV = (2f * Constants.G * attractorMass / satellitePosition.magnitude);
        float escapeV = Mathf.Sqrt(4f * Constants.G * attractorMass / satellitePosition.magnitude);
        _rigidbody2D.velocity += new Vector2(0, initV);
    }
}
