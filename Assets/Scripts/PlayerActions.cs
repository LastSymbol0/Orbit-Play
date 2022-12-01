using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    public Text DebugOrbitText;
    
    private Collider2D _orbitCollider;
    
    void Start()
    {
        var colliders = GetComponents<CircleCollider2D>();

        _orbitCollider = colliders[0].radius > colliders[1].radius ? colliders[0] : colliders[1]; // yeah, very bad
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _orbitCollider.isTrigger = !_orbitCollider.isTrigger;
            
            DebugOrbitText.text = $"Consumption of particles - {_orbitCollider.isTrigger}";
        }
    }
}
