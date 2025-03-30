using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RangeGrunt : MonoBehaviour
{
    //The current behavioral state of it
    public enum states { idle,run,death,shoot}
    public states currentState = states.idle;

    //The animator component
    Animator animator;

    //The current and maximum health
    public float MaxHP;
    [SerializeField] float HP;

    //The navmeshagent component
    NavMeshAgent Agent;

    //The player (used for pathfinding and stuff)
    GameObject player;

    //the name of the dropped ability/ the ability item prefab
    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    //How long it will burn for and how much damage it will take from that burn every second
    float Burntime;
    public float BurnDmgPerSec;

    //How far it can attack from/ the projectile it fires when it shoots
    public float Range;
    public GameObject Projectile;

    //How often it attacks/ how long until the next attack
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

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
        //Setting the hp to the maximum hp
        HP = MaxHP;

        //fetches the navmeshagent and animator components 
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        //Fetching a reference to the player object
        player = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        //creating a new path towards the player
        if (Agent != null && Agent.enabled) 
        {
            Agent.destination = player.transform.position;
        }

        //Applying burn damage
        if (Burntime > 0)
        {
            TakeDamage(BurnDmgPerSec * 0.02f);
            Burntime -= 0.02f;
        }

        //Deincrementing the cooldown if it has yet to pass
        if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }
        //shooting the if the cooldown has passed, the enemy isn't currently dying and the player is within range and visible
        else if (currentState!= states.death && (player.transform.position - transform.position).magnitude <= Range && !Physics.Raycast(transform.position, player.transform.position - transform.position,Range, LayerMask.GetMask("Terrain")) && AttackCooldownCurrent <= 0)
        {
            //changing the state
            currentState = states.shoot;

            //restarting the cooldown
            AttackCooldownCurrent = AttackCooldown;

            //Shooting the projectile after sufficient animation
            Invoke("ShootGun", 0.5f);
        }

        //updating the animations in line with the current state
        if (animator != null) { animator.SetInteger("StateInt", (int)currentState); }
    }

    //This message is sent by other objects to deal this damage (mostly)
    public void TakeDamage(float damage)
    {
        //Reducing the enemie's health
        HP -= damage;

        //If the enemy has died
        if (HP <= 0)
        {
            currentState = states.death;
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

    //Disables the object when it dies;
    void finishDying() 
    {
        gameObject.SetActive(false);
    }

    //Called by other scripts to "ignite" this object
    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }

    //enables the pathfinding after it has shot
    void enablepathfinding() 
    {
        Agent.enabled = true;
        currentState = states.run;
    }

    void ShootGun()
    {
        //disables the pathfinding until it shoots
        Agent.enabled = false;

        //Looking through every gameobject with the right tag to check whether they're disabled
        GameObject[] PotentialProjectiles = GameObject.FindGameObjectsWithTag(Projectile.tag);
        GameObject bullet = Camera.main.gameObject; bool hasFoundBullet = false;
        foreach (var projectile in PotentialProjectiles)
        {
            if (!projectile.transform.GetChild(0).gameObject.activeInHierarchy && !hasFoundBullet)
            {
                bullet = projectile;
                hasFoundBullet = true;
            }
        }

        //if a deactivated shot isn't found a new one is made
        if (!hasFoundBullet)
        {
            bullet = GameObject.Instantiate(Projectile, transform.position, Camera.main.transform.rotation);
            bullet.transform.forward = player.transform.position - transform.position;
            bullet.transform.GetChild(0).gameObject.GetComponent<Collider>().excludeLayers = LayerMask.GetMask("Items", "Enemies");
        }
        //if theres a deactivated shot it is reset and appears to have just been created
        else
        {
            bullet.transform.position = transform.position;
            bullet.transform.forward = player.transform.position - transform.position;
            bullet.transform.GetChild(0).gameObject.SetActive(true);
            bullet.transform.GetChild(0).localPosition = Vector3.zero;
            bullet.transform.GetChild(0).localRotation = Projectile.transform.GetChild(0).localRotation;
            bullet.transform.GetChild(0).gameObject.GetComponent<Collider>().excludeLayers = LayerMask.GetMask("Items", "Enemies");
        }

        //reenables the pathfinding
        Invoke("enablepathfinding", 1f);
    }
}
