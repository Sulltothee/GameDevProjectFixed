using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    //The camera object
    GameObject Camera;

    //the object that stores the direction of the player but flat
    Transform Orientation;

    //The current x and y rotation of the camera 
    float xRotation;
    float yRotation = 0;

    //the sensitivity of the camera in the x and y direction
    public float sensX = 0, sensY = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Getting the camera object
        Camera = GameObject.FindGameObjectWithTag("MainCamera");

        //Makes the mouse Invisible and locks it position
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Gets a reference to the orientation object for movement stuff elsewhere
        Orientation = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        //Setting the rotation of the player
        //Gets the mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        //Camera.transform.position = transform.position + new Vector3(0,-0.75f * Convert.ToInt16(gameObject.GetComponent<PlayerMovement>().State == PlayerMovement.states.Sliding), 0);
        Camera.transform.position = transform.position;

        //Changing the rotation depending on inputs
        yRotation += mouseX;
        xRotation -= mouseY;

        //Limiting the rotation to make sure the player can't do a flip
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Rotate the camera and player
        Camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
