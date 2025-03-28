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

    //How often it attacks
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    //How long it takes for it to attack
    public float PunchTime;

    //Stuff to do with the dropped ability
    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    private void Awake()
    {
        HP = MaxHP;
        Agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponentInChildren<Animator>();

        currentState = states.idle;
    }

    // fixed update is called every 0.02s
    private void FixedUpdate()
    {
        //Enabling/ disabling its behaviors if it can't see the player
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
            AttackCooldownCurrent = AttackCooldown;                                                                                                                                                                                                                                                                  
            currentState = states.Punch;
            Invoke("FinishPunch", PunchTime);
        }
        else if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }

        animator.SetInteger("StateInt", (int)currentState);
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            currentState = states.death;
            Agent.enabled = true;

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

            Invoke("finishDying", 2);
        }
    }

    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }

    public void FinishPunch()
    {
        if ((player.transform.position - transform.position).magnitude <= Range)
        {
            player.SendMessage("TakeDamage", Damage);
        }
        currentState = states.run;
    }

    void finishDying()
    {
        gameObject.SetActive(false);
    }
}

