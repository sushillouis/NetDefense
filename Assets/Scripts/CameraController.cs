using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool fullRotation = true;
    //Variables for adjusting movement speed
        //movementSpeed: speed of translation in XZ plane
        //zoomSpeed: speed of translation on Y axis
        //xMax/zMax: Limit to translation in X and Z directions
        //minZoom/maxZoom: Limits to translation on Y axis
        //speedV/speedH: speed of camera pitch and yaw rotation
    public float movementSpeed, zoomSpeed, xMax, zMax, minZoom, maxZoom, speedV, speedH;

    //Parent objects for translating and rotating
        //arm: parent of all other objects, translated based on movement input
        //yaw: rotated on z axis
        //pitch: rotated on x axis
    public GameObject arm;
    public GameObject yaw;
    public GameObject pitch;

    void FixedUpdate()
    {
        //Checks if any movement input is given
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //Speed multiplied by 0.7 if there is x and z input to control magnitude of movement vector
            float speed = movementSpeed;
            if(Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0){
                speed *= 0.7f;
            }

            //Vector to hold direction of movement in relative space
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed * Time.deltaTime;

            //Checks for zoom input, stores value in float
            float zoom = 0.0f;
            if(Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                zoom = -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * 10 * Time.deltaTime;
            }

            Vector3 translation = Vector3.zero;
            //Next steps unnecessary if XZ movement is 0
            if(movement.magnitude != 0)
            {
                //Movement vector converted to world space in direction camera is facing
                movement = transform.forward * movement.z + transform.right * movement.x;
                //New vector equivalent to XZ component of movement
                translation = new Vector3(movement.x, 0, movement.z);
                //Corrects magnitude of translation vector in XZ plane to total magnitude of movement in 3d space
                translation *= movement.magnitude / translation.magnitude;
            }

            //Adds zoom input determined earlier
            translation += new Vector3(0, zoom, 0);

            //Translates arm appropriately
            arm.transform.Translate(translation, Space.World);
        }

        //Checks if right click is down to begin rotating
        if(Input.GetKey(KeyCode.Mouse1))
        {
            pitch.transform.eulerAngles += new Vector3(-speedV * Input.GetAxis("Mouse Y"), 0, 0);
            if(fullRotation)
            {
                yaw.transform.eulerAngles += new Vector3(0, speedH * Input.GetAxis("Mouse X"), 0);
            }
        }

        //Checks for middle click, resets camera position if found
        if(Input.GetKeyDown(KeyCode.Mouse2))
        {
            pitch.transform.localEulerAngles = new Vector3(60, 0, 0);
            yaw.transform.localEulerAngles = new Vector3(0, 0, 0);
            arm.transform.position = new Vector3(0, 50, 40);
        }
    }
}