using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MeleeGrunt : MonoBehaviour
{
    public float MaxHP;
    public float HP;

    NavMeshAgent Agent;
    GameObject player;

    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    float Burntime;
    public float BurnDmgPerSec;

    public float Range;
    public float Damage;

    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    private void Awake()
    {
        HP = MaxHP;
        Agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (Agent != null) 
        {
            Agent.destination = player.transform.position;
        }

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
        HP -= damage;

        if (HP <= 0) 
        {
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
            gameObject.SetActive(false); 
        }
    }

    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }
}
