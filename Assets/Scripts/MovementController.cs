using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float SpeedScale = 10f;

    public Camera Camera;
    
    private Rigidbody2D _rigidbody2D;
    private Transform _cameraTransform;
    
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _cameraTransform = Camera.GetComponent<Transform>();
    }
    
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 addForce = new Vector2(0f, 0f);
        
        if (horizontal > 0.01f || horizontal < -0.01f)
        {
            addForce.x = horizontal * SpeedScale;
        }
        
        if (vertical > 0.01f || vertical < -0.01f)
        {
            addForce.y = vertical * SpeedScale;
        }
        
        _rigidbody2D.AddForce(addForce, ForceMode2D.Force);
        
        
        _cameraTransform.position = new Vector3(transform.position.x, transform.position.y, _cameraTransform.position.z);
    }
}
