using System.Linq;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    const int NullUserTouchId = -1;



    public float ShootingSpeed = 2f;

    private GameObject Shell;

    private Attractor _attractor;

    private GameObject _joystick;

    private int _uniqueUserTouchId = NullUserTouchId;

    private void Start()
    {
        _attractor = GetComponent<Attractor>();
        _joystick = GameObject.FindGameObjectWithTag("Joystick");
    }

    // Update is called once per frame
    void Update()
    {
        ExecuteUserTouchInput();
    }

    private void ExecuteUserTouchInput()
    {
        if (Input.touchCount > 0)
        {

            foreach (var touch in Input.touches)
            {
                // Did the touch action just begin?
                if (touch.phase == TouchPhase.Began)
                {
                    if(IsTouchInJoystickRect(touch))
                    {
                        if (!(_joystick.activeSelf && Input.touches.Length > 1))
                            continue;
                    }
                    //Select nearest asteroid 
                    _uniqueUserTouchId = touch.fingerId;
                    SelectAsteroid();

                }

                //Shooting
                if (touch.phase == TouchPhase.Ended)
                {
                    if (Shell != null && _uniqueUserTouchId == touch.fingerId)
                    {
                        LookAtMousePosition(Shell);

                        SpriteRenderer renderer = Shell.GetComponent<SpriteRenderer>();
                        if (renderer != null)
                        {
                            renderer.material.SetInt("_Animated", 0);
                        }

                        ShootMetheorite(Shell);

                        Shell = null;
                        _uniqueUserTouchId = NullUserTouchId;
                    }
                }

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

    private void SelectAsteroid()
    {
        if (Shell == null)
        {
            GameObject findedObject = MousePositionToNearestSatellite();

            if (findedObject != null)
            {
                Satellite script = findedObject.GetComponent<Satellite>();

                if (script != null && script.IsOnOrbit)
                {
                    Shell = findedObject;

                    SpriteRenderer renderer = findedObject.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.material.SetInt("_Animated", 1);
                    }
                }
            }
        }
    }

    private void ShootMetheorite(GameObject metheorite)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(metheorite.transform.position);
        Vector2 direction = (Vector2)(Input.mousePosition - screenPoint);

        float shootingForce = Vector2.Distance(screenPoint, Input.mousePosition) * 0.05f;

        direction.Normalize();

        metheorite.GetComponent<Rigidbody2D>().AddForce(direction * shootingForce * ShootingSpeed, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Object look at the mouse position on screen.
    /// </summary>
    private void LookAtMousePosition(GameObject lookingObject)
    {
        Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(lookingObject.transform.position);
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
            return hit.transform.gameObject;
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
            .First();


        return nearestSatellite.satellite.GameObject;
    }

    private bool IsClickInJoystickRect()
    {
        var imgRectTransform = _joystick.GetComponent<RectTransform>();

        Vector2 localMousePosition = Input.mousePosition;//imgRectTransform.InverseTransformPoint(Input.mousePosition);
        if (imgRectTransform.rect.Contains(localMousePosition))
        {
            return true;
        }
        return false;
    }

    private bool IsTouchInJoystickRect(Touch touch)
    {
        var imgRectTransform = _joystick.GetComponent<RectTransform>();

        Vector2 localMousePosition = touch.position;//imgRectTransform.InverseTransformPoint(Input.mousePosition);
        if (imgRectTransform.rect.Contains(localMousePosition))
        {
            return true;
        }
        return false;
    }
}
