using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MeleeGrunt : MonoBehaviour
{
    //The current and maximum health
    public float MaxHP;
    public float HP;

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

    //How far it can attack from and how much damage it deals
    public float Range;
    public float Damage;

    //How often it attacks
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    private void Awake()
    {
        //Setting the hp to the maximum hp
        HP = MaxHP;

        //fetches the navmeshagent component
        Agent = GetComponent<NavMeshAgent>();

        //Fetching a reference to the player object
        player = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        //creating a path to the player
        if (Agent != null) 
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
        if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }
        else if((player.transform.position - transform.position).magnitude <= Range && AttackCooldownCurrent <= 0)
        {
            player.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver);
            AttackCooldownCurrent = AttackCooldown;
        }
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
