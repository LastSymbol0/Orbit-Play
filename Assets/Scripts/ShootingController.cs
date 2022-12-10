using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public float ShootingSpeed = 1f;
    private GameObject Shell;

    // Update is called once per frame
    void Update()
    {
        ExecuteUserInput();

    }

    private void ExecuteUserInput()
    {
        //Get Shell
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Shell == null)
            {
                GameObject findedObject = MousePositionRayToObject();

                if (findedObject != null)
                {
                    if (findedObject.tag != "Player")
                    {
                        Shell = findedObject;
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
                ShootMetheorite(Shell);
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
        int layerObject = 8;
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
}
