using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RangeGrunt : MonoBehaviour
{
    public enum states { idle,run,death,shoot}
    public states currentState = states.idle;
    Animator animator;

    public float MaxHP;
    [SerializeField] float HP;

    NavMeshAgent Agent;
    GameObject player;

    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    float Burntime;
    public float BurnDmgPerSec;

    public float Range;
    public GameObject Projectile;

    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    private void Awake()
    {
        HP = MaxHP;
        Agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponentInChildren<Animator>();    
    }

    private void FixedUpdate()
    {
        if (Agent != null && Agent.enabled) 
        {
            Agent.destination = player.transform.position;
        }

        if (Burntime > 0)
        {
            TakeDamage(BurnDmgPerSec * 0.02f);
            Burntime -= 0.02f;
        }

        if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }
        else if (currentState!= states.death && (player.transform.position - transform.position).magnitude <= Range && Physics.Raycast(transform.position, player.transform.position - transform.position,Range) && AttackCooldownCurrent <= 0)
        {
            currentState = states.shoot;
            AttackCooldownCurrent = AttackCooldown;
            Invoke("ShootGun", 0.5f);
        }

        if (animator != null) { animator.SetInteger("StateInt", (int)currentState); }
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;

        if (HP <= 0) 
        {
            currentState = states.death;

            Agent.enabled = false;

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
            Invoke("finishDying", 1);
        }
    }

    void finishDying() 
    {
        gameObject.SetActive(false);
    }

    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }

    void enablepathfinding() 
    {
        Agent.enabled = true;
        currentState = states.run;
    }

    void ShootGun()
    {
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

        

        Invoke("enablepathfinding", 1f);
    }
}
