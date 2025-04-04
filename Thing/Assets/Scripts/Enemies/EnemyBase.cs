using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    //The current and maximum health
    public float MaxHP;
    public float HP;

    //The player (used for pathfinding and stuff)
    public GameObject player;

    //the name of the dropped ability/ the ability item prefab
    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    //How long it will burn for and how much damage it will take from that burn every second
    public float Burntime;
    public float BurnDmgPerSec;

    //if it's dying
    bool isDying = false;

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
        //Fetching a reference to the player object
        player = GameObject.FindWithTag("Player");

        //Setting the hp to the maximum hp
        HP = MaxHP;

        //Setting the enemy to not be on fire
        Burntime = 0;

        //Setting the enemy to not be dying
        isDying = false;

        //Doing anything the enemy type requires on starting
        SubFreshStart();
    }
    public virtual void SubFreshStart()
    {

    }

    private void FixedUpdate()
    {
        //dealing it burn damage
        if (Burntime > 0)
        {
            TakeDamage(BurnDmgPerSec * 0.02f);
            Burntime -= 0.02f;
        }

        SubFixedUpdate();
    }
    public virtual void SubFixedUpdate()
    {

    }

    public void TakeDamage(float damage)
    {
        //Reducing the enemie's health
        HP -= damage;

        //If the enemy has died
        if (HP <= 0 && !isDying)
        {

            isDying = true;
            DoOnDeath();

            //making the item drop
            if (Name != AbilityType.Names.Empty)
            {
                GameObject[] AllActiveDrops = GameObject.FindGameObjectsWithTag(AbilityDrop.tag);

                bool HasFoundUsable = false;

                for (int i = 0; i < AllActiveDrops.Length; i++)
                {
                    GameObject currentObject = AllActiveDrops[i].transform.GetChild(0).gameObject;
                    if (!currentObject.activeInHierarchy && !HasFoundUsable)
                    {
                        currentObject.SetActive(true);
                        AllActiveDrops[i].transform.position = transform.position;
                        currentObject.GetComponent<AbilityItem>().Type = Name;
                        currentObject.SendMessage("Resety", transform.position.y);

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

    public virtual void DoOnDeath()
    {

    }

    //Called by other scripts to "ignite" this object
    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }
}
