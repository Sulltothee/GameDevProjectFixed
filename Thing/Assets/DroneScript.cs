using UnityEngine;

public class DroneScript : MonoBehaviour
{
    //State machine for animations
    public enum states { Float, death, shoot }
    public states currentState = states.Float;

    //The maximum and current hp
    public float MaxHP;
    [SerializeField] float HP;

    //The name of the ability to be dropped/ the ability item prefab
    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    //The player object
    public GameObject player;

    //How fast this individual moves
    public float Speed;

    //How fare away this stops from the player
    public float StoppingDistance;

    //The rigidbody component
    public Rigidbody rb;

    //How far away from the ground this stays
    public float Groundbuffer = 1.5f;

    //The animator component
    public Animator animator;

    //How long it will burn for and how much damage it will take from that burn every second
    float Burntime;
    public float BurnDmgPerSec;

    //Whether this will continue to pathfind
    public bool DoMovement = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        FreshStart();
    }
    private void OnEnable()
    {
        FreshStart();
    }
    void FreshStart()
    {
        //Setting the hp to the default
        HP = MaxHP;

        //Fetching a reference to the player
        player = GameObject.FindWithTag("Player");

        //fetching the rigidbody component
        rb = GetComponent<Rigidbody>();

        //fetching the animator
        animator = GetComponentInChildren<Animator>();

        //Disabling this object's weight
        rb.useGravity = false;

        //enabling pathfindin
        DoMovement = true;

        //Resetting the hitbox dimensions
        GetComponent<SphereCollider>().radius = 1f;

        //What the subtypes do when loading in
        SubFreshStart();
    }
    //Subtypes of drones will do different things when they load in
    public virtual void SubFreshStart()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Floating into the desired range
        if (DoMovement)
        {
            if ((player.transform.position - transform.position).magnitude > StoppingDistance)
            {
                rb.linearVelocity = (player.transform.position - transform.position).normalized * Speed;
            }
            else if ((player.transform.position - transform.position).magnitude == StoppingDistance)
            {
                rb.linearVelocity = Vector3.zero;
            }
            else
            {
                rb.linearVelocity = (transform.position - player.transform.position).normalized * Speed;
            }
        }

        //Keeping it a certain distance above the ground
        if (currentState != states.death && Physics.Raycast(transform.position, Vector3.down, Groundbuffer, LayerMask.GetMask("Terrain")) && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }

        //Applying burn damage
        if (Burntime > 0)
        {
            TakeDamage(BurnDmgPerSec * 0.02f);
            Burntime -= 0.02f;
        }

        SubFixedUpdate();

        //updating the animations in line with the current state
        if (animator != null) { animator.SetInteger("StateInt", (int)currentState); }
    }

    //Subtypes of drones will do different things routinely
    public virtual void SubFixedUpdate(){}

    //Called by other scripts to "ignite" this object
    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }

    //This message is sent by other objects (mostly)
    public void TakeDamage(float damage)
    {
        //Reducing the enemie's health
        HP -= damage;

        //If the enemy has died
        if (HP <= 0)
        {
            //Setting the state
            currentState = states.death;

            //Disabling movement
            DoMovement = false; rb.linearVelocity = Vector3.zero;

            //Making it fall
            rb.useGravity = true; GetComponent<SphereCollider>().radius = 0.5f;

            //making the item drop
            if (Name != AbilityType.Names.Empty)
            {
                GameObject[] AllActiveDrops = GameObject.FindGameObjectsWithTag(AbilityDrop.tag);

                bool HasFoundUsable = false;

                for (int i = 0; i < AllActiveDrops.Length; i++)
                {
                    GameObject currentObject = AllActiveDrops[i].transform.GetChild(0).gameObject;
                    if (!currentObject.activeInHierarchy)
                    {
                        currentObject.SetActive(true);
                        AllActiveDrops[i].transform.position = transform.position;
                        currentObject.GetComponent<AbilityItem>().Type = Name;

                        HasFoundUsable = true;
                    }
                }
                if (!HasFoundUsable)
                {
                    GameObject currentObject = GameObject.Instantiate(AbilityDrop, transform.position, Quaternion.identity);
                    currentObject.transform.GetChild(0).GetComponent<AbilityItem>().Type = Name;
                }
            }

            //Doing the subtype death tasks
            SubDying();

            //Disabling the object fully after the animation has had time to play
            Invoke("finishDying", 1);
        }
    }

    //Disables the object when it dies
    void finishDying()
    {
        gameObject.SetActive(false);
    }

    public virtual void SubDying() { }

    //Called after a delay to allow the object to wander after stopping to shoot
    void enablepathfinding()
    {
        DoMovement = true;
        if (currentState != states.death) { currentState = states.Float; }
    }
}
