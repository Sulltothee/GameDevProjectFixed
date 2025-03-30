using UnityEngine;
using UnityEngine.AI;

public class ChargerScript : MonoBehaviour
{
    public enum States { Walk, Charge, Idle, Death}
    public States CurrentState = States.Idle;

    Rigidbody rb;

    public NavMeshAgent Agent;

    //The player (used for pathfinding and stuff)
    GameObject player;

    //the name of the dropped ability/ the ability item prefab
    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    //How long it will burn for and how much damage it will take from that burn every second
    float Burntime = 0;
    public float BurnDmgPerSec;

    //The current and maximum health
    public float MaxHP;
    public float HP;

    //How far away it can see you
    public float SeeDistance = 100;

    //How close it has to be to the player to charge;
    public float ChargeDistance = 10;

    //How fast it charges
    public float ChargeSpeed = 10;

    //What direction it's charging in
    Vector3 ChargeDirection;

    //How tall this enemy is
    public float height = 4;

    //How long this will charge before giving up
    public float ChargeTime = 10;

    //Whether this can charge
    public bool CanCharge = false;

    //How much time has to pass between one charge ending and another beginning
    public float ChargeInterval = 3;

    //How much force is applied to the player when they collide with a charge in the x and z/ y axis
    public float PushStrengthFlat = 1;
    public float PushStrengthY = 1;

    //How much damage this deals when colliding with it during a charge
    public float Damage = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        FreshStart();
    }
    private void OnEnable()
    {
        FreshStart();
    }
    void FreshStart()
    {
        rb = GetComponent<Rigidbody>();

        Agent = GetComponent<NavMeshAgent>();

        CurrentState = States.Idle;

        player = GameObject.FindWithTag("Player");

        HP = MaxHP;

        Burntime = 0;

        ChargeDirection = Vector3.zero;

        CanCharge = false;

        Invoke("SetChargeable", ChargeInterval);
    }

    private void FixedUpdate()
    {
        //Determining whether it can see the player
        bool CanSeePlayer = !(Physics.Raycast(transform.position, player.transform.position - transform.position, (player.transform.position - transform.position).magnitude, LayerMask.GetMask("Terrain")) || (player.transform.position - transform.position).magnitude > SeeDistance);

        //Enabling it if it can see the player
        if (CanSeePlayer && CurrentState == States.Idle)
        { CurrentState = States.Walk; }

        //Making it walk towards the player
        if(Agent != null && Agent.enabled && Agent.isOnNavMesh && CurrentState == States.Walk)
        {
            Agent.destination = player.transform.position;
        }

        //Charging if it sees and is close enough to the player
        if(CanSeePlayer && Vector3.Scale((player.transform.position - transform.position), new Vector3(1,0,1)).magnitude < ChargeDistance && CurrentState == States.Walk && CanCharge)
        {
            Agent.enabled = false;
            ChargeDirection = Vector3.Scale((player.transform.position - transform.position), new Vector3(1, 0, 1)).normalized;
            Invoke("StartCharge", 2f);
        }
        if (CurrentState == States.Charge)
        { rb.linearVelocity = ChargeDirection * ChargeSpeed + Vector3.Scale(rb.linearVelocity, Vector3.up); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Terrain") && collision.GetContact(0).point.y > transform.position.y - height/2 && CurrentState == States.Charge)
        {
            CurrentState = States.Walk;
            rb.linearVelocity = Vector3.zero;

            if (Agent.isOnNavMesh)
            {
                Agent.enabled = true;
            }
            Invoke("SetChargeable", ChargeInterval);


        }
        else if (collision.gameObject.CompareTag("Player") && CurrentState == States.Charge)
        {
            player.GetComponent<Rigidbody>().AddForce((Vector3.Scale((player.transform.position - transform.position), new Vector3(1, 0, 1)).normalized * PushStrengthFlat) + (Vector3.up * PushStrengthY));
            player.SendMessage("TakeDamage", Damage);
        }
    }

    void StartCharge()
    {
        CurrentState = States.Charge;
        Invoke("StopCharge", ChargeTime);
    }

    void StopCharge()
    {
        if (CurrentState == States.Charge)
        {
            CurrentState = States.Walk;
            rb.linearVelocity = Vector3.zero;
            if (Agent.isOnNavMesh)
            {
                Agent.enabled = true;
            }
            Invoke("SetChargeable", ChargeInterval);
        }
    }

    void SetChargeable()
    {
        CanCharge = true;
    }

    public void TakeDamage(float damage)
    {
        //Reducing the enemie's health
        HP -= damage;

        //If the enemy has died
        if (HP <= 0)
        {
            Agent.enabled = false;

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

            //Disabling the object after it dies
            gameObject.SetActive(false);
        }
    }

    //Called by other scripts to "ignite" this object
    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }
}
