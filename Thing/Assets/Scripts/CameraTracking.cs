using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    GameObject Camera;
    Transform Orientation;

    float xRotation;
    float yRotation = 1000;

    public float sensX = 0, sensY = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Getting the camera object
        Camera = GameObject.FindGameObjectWithTag("MainCamera");

        //Makes the mouse Invisible and locks it position
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Orientation = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        //Setting the rotation of the player
        //Gets the mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        Camera.transform.position = transform.position;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Rotate the camera and player
        Camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
