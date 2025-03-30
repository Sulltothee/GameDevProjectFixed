using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RailGunGuy : MonoBehaviour
{
    //State machine for animations
    public enum states { idle,run,death,shoot}
    public states currentState = states.idle;

    //the animator component
    Animator animator;

    //The maximum and current hp
    public float MaxHP;
    [SerializeField] float HP;

    //The navmesh agent component used for navigation
    NavMeshAgent Agent;

    //The player object
    GameObject player;

    //The name of the ability to be dropped/ the ability item prefab
    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    //how long this will burn for/ how much burn damage it takes every second
    float Burntime;
    public float BurnDmgPerSec;

    //How far it can shoot you from
    public float Range;

    //How long between shots/ how long until the next shot
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    //How much damage a hit deals
    public float Damage = 1;

    //The last position of the player, allows them to "dodge" the shot
    Vector3 LastKnownPlayerPosition = Vector3.zero;

    //The renderer component for the laser
    LineRenderer lineRenderer;

    //Runs whenever the script is loaded
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
        //Sets the hp to the maximum hp
        HP = MaxHP;

        //fetches the navmeshagent, animator and line renderer components
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        lineRenderer = GetComponent<LineRenderer>();

        //fetches the player object
        player = GameObject.FindWithTag("Player");

        //Reenabling the laser
        lineRenderer.enabled = true;
    }


    private void FixedUpdate()
    {
        //If the navmeshagent isn't disabled, make a path towards the player
        if (Agent != null && Agent.enabled) 
        {
            Agent.destination = player.transform.position;
        }

        //Dealing damage if the enemy is on fire
        if (Burntime > 0)
        {
            TakeDamage(BurnDmgPerSec * 0.02f);
            Burntime -= 0.02f;
        }

        //Decreasing the cooldown for the next attack
        if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }

        //Attacking
        else if (currentState!= states.death && (player.transform.position - transform.position).magnitude <= Range && Physics.Raycast(transform.position, player.transform.position - transform.position,Range) && AttackCooldownCurrent <= 0)
        {
            //Changing the state
            currentState = states.shoot;

            //Restarting the cooldown
            AttackCooldownCurrent = AttackCooldown;

            //Setting the lastknown player position so it can miss the shot
            LastKnownPlayerPosition = player.transform.position;

            //Shooting the shot after 0.5 seconds have passed
            Invoke("ShootGun", 0.5f);
        }

        //applying the current state to the animator
        if (animator != null) { animator.SetInteger("StateInt", (int)currentState); }

        //Laser stuff
        //Changing the last known player position if the enemy isn't shooting so you can see where it's aiming towards
        LastKnownPlayerPosition = Vector3.Lerp(LastKnownPlayerPosition, player.transform.position, 0.1f); 

        //Determining the laser's end point
        RaycastHit[] allHits = Physics.RaycastAll(transform.position, LastKnownPlayerPosition - transform.position, Range, LayerMask.GetMask("Terrain", "Player"));
        Vector3 point2;
        if (allHits.Length > 0) { point2 = allHits[0].point; }
        else { point2 = transform.position + ((LastKnownPlayerPosition - transform.position).normalized * Range); }

        //Setting the start and end points of the lasert
        if (lineRenderer != null && lineRenderer.enabled) { lineRenderer.SetPositions(new Vector3[] { transform.position, point2 }); }
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

            //Disabling the laser and pathfinding
            lineRenderer.enabled = false;
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

            //Disabling the object fully after the animation has had time to play
            Invoke("finishDying", 1);
        }
    }

    //Disables the object when it dies
    void finishDying() 
    {
        gameObject.SetActive(false);
    }

    //Called by other objects to "ignite" this object
    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }

    //Called after a delay to allow the object to wander after stopping to shoot
    void enablepathfinding() 
    {
        Agent.enabled = true;
        currentState = states.run;
    }

    //Called to shoot the gun
    void ShootGun()
    {
        //disabling the movement (temporarily)
        Agent.enabled = false;

        //Dealing damage to the player if they are within the laser
        RaycastHit[] hits = Physics.RaycastAll(transform.position, LastKnownPlayerPosition - transform.position, Range, LayerMask.GetMask("Terrain", "Player"));
        if (hits.Length > 0 && hits[0].collider.gameObject.tag == "Player")
        {
            player.SendMessage("TakeDamage", Damage);
        }

        //enabling the pathfinding after a delay
        Invoke("enablepathfinding", 1f);
    }
}
