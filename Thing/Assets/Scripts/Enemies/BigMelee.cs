using UnityEngine;
using UnityEngine.AI;

public class BigMelee : MonoBehaviour
{
    //The current behavioral state of it
    public enum states { idle, run, death, Punch }
    public states currentState = states.idle;

    //How close you have to be to be "seen"
    public float SeeDistance;

    //The animator component
    Animator animator;

    //The navmeshagent component
    NavMeshAgent Agent;

    //The current and maximum health
    float HP;
    public float MaxHP;

    //The player (used for pathfinding and stuff)
    GameObject player;

    //How long it will burn for and how much damage it will take from that burn every second
    float Burntime;
    public float BurnDmgPerSec;

    //How far it can attack from and how much damage it deals
    public float Range = 6;
    public float Damage;

    //How often it attacks/ how long until the next attack
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    //How long it takes for it to attack
    public float PunchTime;

    //the name of the dropped ability/ the ability item prefab
    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    private void Awake()
    {
        //Setting the hp to the maximum hp
        HP = MaxHP;

        //fetches the navmeshagent and animator components 
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        //Fetching a reference to the player object
        player = GameObject.FindWithTag("Player");
    }

    // fixed update is called every 0.02s
    private void FixedUpdate()
    {
        //Enabling/ disabling its behaviors if it can't see the player or if it's dying
        if(currentState != states.death && currentState!= states.Punch && !Physics.Raycast(transform.position, player.transform.position - transform.position, SeeDistance, -player.layer )) {currentState = states.idle;} 
        else if (currentState != states.death && currentState != states.Punch) { currentState = states.run;}
        if (Agent != null && Agent.enabled && currentState == states.run)
        {
            Agent.destination = player.transform.position;
        }
        
        //dealing it burn damage
        if (Burntime > 0)
        {
            TakeDamage(BurnDmgPerSec * 0.02f);
            Burntime -= 0.02f;
        }

        //Attacking
        if(AttackCooldownCurrent <= 0 && (player.transform.position - transform.position).magnitude <= Range) 
        {
            //restarting the cooldown
            AttackCooldownCurrent = AttackCooldown;

            //changing the state
            currentState = states.Punch;

            //finishing the punch after the animation has played some and the player has had a chance to run
            Invoke("FinishPunch", PunchTime);
        }
        //decreasing the attack cooldown if it hasn't passed
        else if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }

        //Updating the animations in reference to the current state
        animator.SetInteger("StateInt", (int)currentState);
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
            Invoke("finishDying", 2);
        }
    }

    //Called by other scripts to "ignite" this object
    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }

    //called to reenable the pathfinding and deal damage to the player at the end of a punch
    public void FinishPunch()
    {
        //Deals damage to the player if they're still in range
        if ((player.transform.position - transform.position).magnitude <= Range)
        {
            player.SendMessage("TakeDamage", Damage);
        }
        currentState = states.run;
    }

    //Disables the object when it dies;
    void finishDying()
    {
        gameObject.SetActive(false);
    }
}

