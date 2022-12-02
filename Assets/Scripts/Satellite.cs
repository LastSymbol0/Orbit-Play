using System;
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

    public bool IsOnOrbit = false;
    
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

    public void UpdateOrbitalMovement_old()
    {
        // Vector3 attractorPosition = _currentAttractorTransform.position;
        // Vector3 satellitePosition = transform.position;
        // float attractorMass = _currentAttractorRigidbody2D.mass;
        // float satelliteMass = _rigidbody2D.mass;
        //
        // bool addExtraRotation = _rigidbody2D.velocity.magnitude < 3f;
        //
        // if (addExtraRotation)
        // {
        //     Debug.Log("Rotating");
        // }
        //
        // float r = Vector3.Distance(attractorPosition, satellitePosition);
        // float totalForce = -(Constants.G * attractorMass * satelliteMass) / (r * r);
        // Vector2 direction = (satellitePosition - attractorPosition);
        // Vector2 additionalRotationForce = addExtraRotation ? transform.up : Vector2.zero;
        // Vector2 force = (direction + additionalRotationForce).normalized * totalForce;
        //
        // // move
        // _rigidbody2D.AddForce(force);
        //
        // //rotate to "look" on the attractor
        // transform.right = attractorPosition - satellitePosition;
    }

    public void UpdateOrbitalMovement()
    {
        Vector3 attractorPosition = _currentAttractorTransform.position;
        Vector3 satellitePosition = transform.position;
        float attractorMass = _currentAttractorRigidbody2D.mass;
        float satelliteMass = _rigidbody2D.mass;
        
        
        float DBG_RotationSpeedScale = currentAttractor.DBG_RotationSpeedScale;
        float DBG_RotationSpeedLimit = currentAttractor.DBG_RotationSpeedLimit;
        float DBG_OrbitDistance = currentAttractor.OrbitDistance;


        Vector2 finalForce = Vector2.zero;
        
        if (!IsOnOrbit) // just a common gravitation
        {
            bool addExtraRotation = _rigidbody2D.velocity.magnitude < 3f;

            if (addExtraRotation)
            {
                // Debug.Log("Rotating");
            }
            
            float r = Vector3.Distance(attractorPosition, satellitePosition);
            float totalForce = -(Constants.G * attractorMass * satelliteMass) / (r * r);
            Vector2 direction = (satellitePosition - attractorPosition);
            Vector2 additionalRotationForce = addExtraRotation ? transform.up : Vector2.zero;
            finalForce = (direction + additionalRotationForce).normalized * totalForce;
        }
        else  // trying to move by orbit
        {
            var distance = (satellitePosition - attractorPosition).magnitude;

            var regulateDistanceToAttractorForce = distance - DBG_OrbitDistance;


            // if (distance <= DBG_OrbitDistance)
            // {
            //     _rigidbody2D.velocity = Vector2.zero;
            // }
            if (_rigidbody2D.velocity.magnitude < DBG_RotationSpeedLimit)
            {
                float r = Vector3.Distance(attractorPosition, satellitePosition);
                float totalForce = (-(Constants.G * attractorMass * satelliteMass) / (r * r));
                
                // totalForce *= Mathf.Abs(regulateDistanceToAttractorForce);
                //
                //
                // if (Mathf.Abs(totalForce) > currentAttractor.DBG_PushOutForceLimit)
                // {
                //     totalForce = totalForce > 0 ?
                //         currentAttractor.DBG_PushOutForceLimit
                //         : -currentAttractor.DBG_PushOutForceLimit;
                // }
                
                Vector2 directionClockwise = transform.up * DBG_RotationSpeedScale;
                Vector2 directionToAttractor = -transform.right * regulateDistanceToAttractorForce;
                finalForce = (directionToAttractor + directionClockwise).normalized * totalForce;
            }
        }
        
                
        //rotate to "look" on the attractor
        transform.right = attractorPosition - satellitePosition;

        // move
        _rigidbody2D.AddForce(finalForce);
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
