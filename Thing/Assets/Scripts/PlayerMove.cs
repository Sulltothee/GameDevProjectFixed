using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Tooltip("Quantity of Force applied to player when they Jump")] public float JumpStrength;
    [Tooltip("How much force is applied to the player every second")] public float MovementSpeed;
    [Tooltip("the Height of the player")] public float height;

    //The rigidbody of the player
    Rigidbody rb;

    //the object that tracks what direction the player should move in reference to it being forwards
    Transform Orientation;

    //The cooldowns relating to the dash
    public float DashCooldownMax;
    float DashCooldownCurrent = 0;
    public float DashForce;

    // Start is called before the first frame update
    void Start()
    {
        //Getting the rigidbody of the player
        rb = GetComponent<Rigidbody>();

        //Getting the object that stores the orientation of movement for the better
        Orientation = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        //Calling the method for moving the player
        MovePlayer();

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, height * 0.5f);

        if (DashCooldownCurrent > 0) { DashCooldownCurrent -= Time.deltaTime; }

        //If the player is on the ground and they have pressed space this frame, they jump
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * JumpStrength);
        }
        //Otherwise if they're not on the ground and the dash cooldown has passed, the player dashes in the direction they're facing
        else if (!isGrounded && DashCooldownCurrent <= 0 && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Camera.main.transform.forward * DashForce);
            DashCooldownCurrent = DashCooldownMax;
        }
    }

    void MovePlayer()
    {
        //moves the player by adding force (good)
        Vector3 moveDirection = Orientation.forward * Input.GetAxisRaw("Vertical") + Orientation.right * Input.GetAxisRaw("Horizontal");
        rb.AddForce(MovementSpeed * moveDirection.normalized * Time.deltaTime);
    }
}
