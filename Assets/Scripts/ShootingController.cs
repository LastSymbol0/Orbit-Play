using System.Linq;
using Mirror;
using UnityEngine;

public class ShootingController : NetworkBehaviour
{
    public float ShootingSpeed = 1f;
    private GameObject Shell;

    private Attractor _attractor;

    private GameObject _joystick;

    private void Start()
    {
        _attractor = GetComponent<Attractor>();
        _joystick = GameObject.FindGameObjectWithTag("Joystick");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        
        ExecuteUserInput();
    }

    private void ExecuteUserInput()
    {
        //Get Shell
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Shell == null)
            {
                GameObject findedObject = MousePositionToNearestSatellite();
                
                if (findedObject != null)
                {
                    // if (findedObject.tag != "Player")
                    {
                        Debug.Log("dew - 1");
                        Satellite script = findedObject.GetComponent<Satellite>();

                        if(script != null && script.IsOnOrbit)
                        {
                            Debug.Log("dew - 2");

                            Shell = findedObject;
        
                            Shell.GetComponent<SatelliteAnimationController>().SetCharged();

                            // SpriteRenderer renderer = findedObject.GetComponent<SpriteRenderer>();
                            // if (renderer != null)
                            // {
                            //     renderer.material.SetInt("_Animated", 1);
                            // }
                        }
                    }
                }
            }

        }

        //Shooting
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (Shell != null)
            {

                LookAtMousePosition(Shell);

                SpriteRenderer renderer = Shell.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.material.SetInt("_Animated", 0);
                }

                ShootMeteorite(Shell);

                Shell = null;
            }
        }

        //Remove it now, rotation change gravity force

        ////Hold mouse button down
        //else if (Input.GetKey(KeyCode.Mouse0))
        //{
        //    if (Shell != null)
        //    {
        //        LookAtMousePosition(Shell);
        //    }
        //}
    }
    
    private void ShootMeteorite(GameObject meteorite)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(meteorite.transform.position);
        Vector2 direction = (Vector2)(Input.mousePosition - screenPoint);

        float shootingForce = Vector2.Distance(screenPoint, Input.mousePosition) * 0.05f;

        direction.Normalize();

        ShootMeteorite_Cmd(meteorite, direction, shootingForce);
    }


    /// <summary>
    /// Object look at the mouse position on screen.
    /// </summary>
    private void LookAtMousePosition(GameObject lookingObject)
    {
        Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(lookingObject.transform.position);

        LookInDirection_Cmd(lookingObject, direction);
    }

    [Command]
    private void ShootMeteorite_Cmd(GameObject meteorite, Vector2 direction, float shootingForce)
    {
        meteorite.GetComponent<Rigidbody2D>().AddForce(direction * (shootingForce * ShootingSpeed), ForceMode2D.Impulse);
        
        meteorite.GetComponent<SatelliteAnimationController>().SetSatellite();
    }

    [Command]
    private void LookInDirection_Cmd(GameObject lookingObject, Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        lookingObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }


    /// <summary>
    /// Get GameObject under mouse position on screen.
    /// </summary>
    /// <returns></returns>
    private GameObject MousePositionRayToObject()
    {
        int layerObject = 0;
        Vector2 ray = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.Raycast(ray, ray, layerObject);

        if (hit.collider != null)
        {
            Debug.Log("Hit the object. Object name is - " + hit.transform.name);

            return hit.transform.gameObject;
        }
        else
        {
            Debug.LogError("Raycast hit no one. Can't get GameObject.");
        }

        return null;
    }

    private GameObject MousePositionToNearestSatellite()
    {
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var nearestSatellite = _attractor.Satellites
            .Select(x => new
            {
                satellite = x,
                dist = Vector3.Distance(x.GameObject.transform.position, mouseWorldPoint)
            })
            .OrderBy(x => x.dist)
            .FirstOrDefault();


        return nearestSatellite?.satellite?.GameObject;
    }
}
