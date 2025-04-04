using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //Health stuff
    public float HP;
    float Burntime;
    public float BurnDmgPerSec;
    public float InvulnerabilityTime;
    float CurrentInvulnerabilityTime = 0;

    //Movement variables
    [Tooltip("Quantity of Force applied to player when they Jump")] public float JumpStrength;
    [Tooltip("How much force is applied to the player every second")] public float MovementSpeed;
    [Tooltip("the Height of the player")] public float height;
    [Tooltip("what the movement speed is multiplied by when sprinting")] public float SprintModifier = 1.5f;

    //default values
    float DefaultMoveSpeed;
    float DefaultMass;

    //The rigidbody of the player
    Rigidbody rb;

    //the object that tracks what direction the player should move in reference to it being forwards
    Transform Orientation;

    //The cooldowns relating to the dash
    [Tooltip("The minimum amount of time that passes between dashes")] public float DashCooldownMax;
    float DashCooldownCurrent = 0;
    [Tooltip("The amount of force applied to the player when they dash")] public float DashForce;

    //The current State of the player
    public enum states { Walking, Sprinting, Sliding, WallRunning }
    public states State;

    //If the player is on the ground
    bool isGrounded;

    //WallJumping
    Vector3 WallJumpAwayFrom;
    bool Onwall;

    //The player's collider
    CapsuleCollider collide;

    //Test Stuff
    public bool TestBool;

    // Start is called before the first frame update
    void Start()
    {
        //Getting the rigidbody of the player
        rb = GetComponent<Rigidbody>();

        //Getting the object that stores the orientation of movement for the better
        Orientation = transform.GetChild(0);

        //fetching the collider component
        collide = GetComponent<CapsuleCollider>();

        //Setting defaults
        DefaultMoveSpeed = MovementSpeed;
        DefaultMass = rb.mass;
    }

    // Update is called once per frame
    void Update()
    {
        //Reducing the player's invincibility timer
        if (CurrentInvulnerabilityTime > 0) { CurrentInvulnerabilityTime -= Time.deltaTime; }

        //Calling the method for moving the player
        MovePlayer();

        //Checking whether the player is grounded 
        isGrounded = Physics.Raycast(transform.position, Vector3.down, height * 0.5f, LayerMask.GetMask("Terrain"));

        //Decreasing the cooldown for dashing
        if (DashCooldownCurrent > 0) { DashCooldownCurrent -= Time.deltaTime; }

        //Jumping and Dashing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //If the player is on the ground and they have pressed space this frame, they jump
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * JumpStrength);
                State = states.Walking;
            }
            //If the player is on a wall they walljump
            else if (Onwall)
            {
                rb.AddForce((transform.position - WallJumpAwayFrom).normalized * JumpStrength * 1.3f);
                State = states.Walking;
            }
            //if the player is not on the ground and the dash timer has passed, they dash
            else if (DashCooldownCurrent <= 0)
            {
                rb.AddForce(Camera.main.transform.forward * DashForce);
                DashCooldownCurrent = DashCooldownMax;
                State = states.Walking;
            }
        }

        //Sliding
        if (isGrounded && Input.GetKeyDown(KeyCode.LeftControl) && rb.linearVelocity.magnitude > 1 && State != states.Sliding)
        {
            transform.position -= Vector3.up * 0.5f;
            collide.height = 1;
            State = states.Sliding;
        }
        //If the player is sliding too slow or manually stops sliding
        else if((Input.GetKeyDown(KeyCode.LeftControl) || rb.linearVelocity.magnitude < 0.5f) && State == states.Sliding)
        {
            State = states.Walking;
        }

        //Running
        if (isGrounded && Input.GetKeyDown(KeyCode.LeftShift) && State != states.Sprinting)
        {
            State = states.Sprinting;
            MovementSpeed *= 2;
        }
        //If the player is Running too slow or manually stops sliding
        else if ((Input.GetKeyUp(KeyCode.LeftShift)) && State == states.Sprinting)
        {
            State = states.Walking;
        }

        //Setting the defaults 
        if(State != states.Sprinting && MovementSpeed != DefaultMoveSpeed) {MovementSpeed = DefaultMoveSpeed;}
        if(State != states.Sliding && collide.height == 1)
        {
            transform.position += Vector3.up * 0.5f;
            collide.height = 2;
        }
    }

    private void FixedUpdate()
    {
        //Applying burn damage
        if(Burntime > 0)
        {
            HP -= BurnDmgPerSec * 0.02f;
            Burntime -= 0.02f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //WallJumping
        if (collision.gameObject.tag == "Terrain" && !isGrounded)
        {
            WallJumpAwayFrom = collision.GetContact(0).point;
            Onwall = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //WallJumping
        if (collision.gameObject.tag == "Terrain" && !isGrounded)
        {
            WallJumpAwayFrom = collision.GetContact(0).point;
            Onwall = true;
        }
    }

    private void OnCollisionExit(Collision collision) 
    { 
        //WallJumping
        if (collision.gameObject.tag == "Terrain")
        {
            Onwall = false;
        }
    }

    void MovePlayer()
    {
        //moves the player by adding force (good)
        Vector3 moveDirection = Orientation.forward * Input.GetAxisRaw("Vertical") + Orientation.right * Input.GetAxisRaw("Horizontal");

        //if the player is moving too slow this speeds them up more
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) { rb.linearVelocity = Vector3.Scale(rb.linearVelocity, new Vector3(0.5f, 1, 0.5f))  ; rb.AddForce(moveDirection.normalized * 200); }
      
        else { rb.AddForce(MovementSpeed * moveDirection.normalized * Time.deltaTime * Convert.ToInt16(State != states.Sliding)); }
    }

    //This is called by other objects to deal damage to the player
    public void TakeDamage(float damage)
    {
        //Taking the damage
        HP -= damage * Convert.ToInt16(CurrentInvulnerabilityTime <= 0) ;

        //If the player is dead the level restarts
        if(HP <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //Making the player invulnerable
        if (CurrentInvulnerabilityTime <= 0) { CurrentInvulnerabilityTime = InvulnerabilityTime; }
    }

    //this is called by other objects to "ignite" the player
    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }
}
