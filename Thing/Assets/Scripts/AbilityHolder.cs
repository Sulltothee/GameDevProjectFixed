using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class AbilityHolder : MonoBehaviour
{
    //How Many abilities have been implemented
    public int NumOfAbilities;

    //all the projectiles for the weapons
    public GameObject[] Projectiles;

    //All the possible Types of abilities
    AbilityType[] AbilityTypes;

    //This points to the index of the currently selected abilities
    public int AbilityIndex = 0;   

    //The currently Held abilities
    AbilityInstance[] CurrentAbilities;

    //The UI icons for each ability
    [Tooltip("The UI icons for each ability")]
    public Sprite[] Icons;

    //The UI icon for an empty ability slot
    [Tooltip("The UI icon for an empty ability slot")]
    public Sprite EmptyIcon;

    //The gameobjects in the ui that display the abilities
    [Tooltip("The gameobjects in the ui that display the abilities")]
    public GameObject[] IconSlots;

    //The amount of seconds left until each respective ability can be activated again
    float[] coolDowns;

    //The amount of charge held for the current weapon (used for abilities like the railgun)
    public float CurrentCharge = 0;

    //The default/ current punch damage
    public float PunchDamageDefault = 1;
    float PunchDamage = 1;

    //The maximum range for the punch
    public float MaxRange = 1;

    //How long between punches/ how long until the next punch can be performed
    public float PunchCooldown = 1;
    float PunchCooldownCurrent = 0;

    //The initial abilities the player will have
    public AbilityType.Names[] InitialInventory;

    // Start is called before the first frame update
    void Start()
    {
        //Setting the punch damage to the default
        PunchDamage = PunchDamageDefault;

        //Instantializing the ability slots
        CurrentAbilities = new AbilityInstance[4];
        for (int i = 0; i < 4; i++)
        {
            CurrentAbilities[i] = new AbilityInstance(new Empty());
        }

        //Assigning all the abilities their respective info
        AbilityTypes = new AbilityType[NumOfAbilities];
        //Pistol
        AbilityTypes[0] = new ProjectileWeapon(AbilityType.Names.Pistol, Projectiles[0], Icons[0]); 
        //Grenade
        AbilityTypes[1] = new ProjectileWeapon(AbilityType.Names.Grenade, Projectiles[1], Icons[1]); 
        //Sticky Grenade
        AbilityTypes[2] = new ProjectileWeapon(AbilityType.Names.StickyNade, Projectiles[2], Icons[2]); 
        //Molotov Cocktail
        AbilityTypes[3] = new ProjectileWeapon(AbilityType.Names.Molotov, Projectiles[3], Icons[3]); 
        //Sniper Rifle
        AbilityTypes[4] = new RayCastGun(AbilityType.Names.SniperRifle, Icons[4]); 
        //Automatic Rifle
        AbilityTypes[5] = new RayCastGun(AbilityType.Names.AutoRifle, Icons[5]); 
        //Railgun
        AbilityTypes[6] = new RailGun(Icons[6]); 
        //Grenade Launcher
        AbilityTypes[7] = new ProjectileWeapon(AbilityType.Names.GrenadeLauncher, Projectiles[4], Icons[7]);
        //Big punch
        AbilityTypes[8] = new BigPunch(Icons[8]);

        //Instantializing the cooldowns
        coolDowns = new float[4]; 

        //Test Stuff, Giving the player example abilities
        for (int i = 0; i < 4 ; i++)
        {
            if (InitialInventory[i] != AbilityType.Names.Empty)
            {
                CurrentAbilities[i] = new AbilityInstance(AbilityTypes[(int)InitialInventory[i] - 1]);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Activating the ability (if the cooldown has passed and it has uses left) based on a variety of conditions depending on the activation type of the ability (if the mouse was pressed this frame for press type, if the mouse is being held for Auto type, the mouse was released for charge types)
        if (((Input.GetKeyDown(KeyCode.Mouse0) && CurrentAbilities[AbilityIndex].abilityType.ActivationType == AbilityType.ActivationTypes.Press) || (Input.GetKey(KeyCode.Mouse0) && CurrentAbilities[AbilityIndex].abilityType.ActivationType == AbilityType.ActivationTypes.Auto) || (Input.GetKeyUp(KeyCode.Mouse0) && CurrentAbilities[AbilityIndex].abilityType.ActivationType == AbilityType.ActivationTypes.Charge)) && CurrentAbilities[AbilityIndex].currentUses > 0 && coolDowns[AbilityIndex] <= 0) {
            CurrentAbilities[AbilityIndex].Activate();
            if (CurrentAbilities[AbilityIndex].currentUses > 0) { coolDowns[AbilityIndex] = CurrentAbilities[AbilityIndex].abilityType.Cooldown; }
        }
        //if the mouse is being held and the current ability is charge type, the current charge is increased
        else if((Input.GetKey(KeyCode.Mouse0) && CurrentAbilities[AbilityIndex].abilityType.ActivationType == AbilityType.ActivationTypes.Charge))
        {
            CurrentCharge += Time.deltaTime; Math.Clamp(CurrentCharge, 0, 1);
        }
        
        //Scroll wheel changing what ability is selected and decreasing charge
        if(Input.mouseScrollDelta.y < 0) { AbilityIndex++; if (AbilityIndex > 3) { AbilityIndex = 0; CurrentCharge = 0; } }
        else if (Input.mouseScrollDelta.y > 0) { AbilityIndex--; if (AbilityIndex < 0) { AbilityIndex = 3; CurrentCharge = 0; } }

        //The Punch
        if(Input.GetKeyDown(KeyCode.V) && PunchCooldownCurrent <= 0) 
        {
            //Checking everything it can hit within the range
            RaycastHit[] Hits = Physics.RaycastAll(transform.position, Camera.main.transform.forward, MaxRange, LayerMask.GetMask("Terrain", "Enemies"));

            //Dealing those collided objects until it hits terrain or the list is empty
            bool HasHitTerrain = false;
            int HitIndex = 0;
            while (!HasHitTerrain && HitIndex < Hits.Length)
            {
                if (Hits[HitIndex].collider.gameObject.tag == "Terrain")
                {
                    HasHitTerrain = true;
                }

                Hits[HitIndex].collider.gameObject.SendMessage("TakeDamage", PunchDamage, SendMessageOptions.DontRequireReceiver);
                HitIndex++;
            }
            //emptying the charge
            GetComponent<AbilityHolder>().CurrentCharge = 0;
            //resetting the cooldown to punch
            PunchCooldownCurrent = PunchCooldown;
        }
        //if the punch cooldown hasn't passed it is decreased
        else if (PunchCooldownCurrent>0)
        {
            PunchCooldownCurrent -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        //For every equipped ability...
        for (int i = 0; i < 4; i++)
        {
            //if the ability is empty the sprite is set to empty and white
            if (CurrentAbilities[i].abilityType.name == AbilityType.Names.Empty)
            { IconSlots[i].GetComponent<Image>().sprite = EmptyIcon; IconSlots[i].GetComponent<Image>().color = Color.white; }

            else 
            {
                //if the ability is NOT empty the sprite is set to the relevant sprite for the ability
                IconSlots[i].GetComponent<Image>().sprite = CurrentAbilities[i].Icon;

                //if the cooldown is not over the ability icon is set to black (change this later)
                if (coolDowns[i]>0) { IconSlots[i].GetComponent<Image>().color = Color.black; coolDowns[i] -= 0.2f; }

                //if the cooldown is over the color is set to white
                else { IconSlots[i].GetComponent<Image>().color = Color.white; }
            }
            
        }
    }

    //Called by other objects to give this abilities
    public bool GiveAbility(AbilityType.Names abilityType)
    {
        //Checking if there's an empty slot
        int AbilityIndex = -1;
        for(int i = 0;i < 4; i++)
        {
            if (CurrentAbilities[i].abilityType.name == AbilityType.Names.Empty)
            {
                AbilityIndex = i;
                break;
            }
        }

        //if theres an empty slot the ability is gained
        if(AbilityIndex > -1) { CurrentAbilities[AbilityIndex] = new AbilityInstance(AbilityTypes[(int)abilityType - 1]); return true; }
        //if there isnt the function returns false (used to not destroy ability items when they haven't been harvested)
        else return false;
    }
}

//This class holds the basic components necessary for every ability type
public abstract class AbilityType
{
    //The image representing the ability
    public Sprite Icon;

    //The player object, this stops the need to frequently find the player which will make unnesescary load
    public GameObject Player;

    //The names of every ability type/ the name of the instance (default empty)
    public enum Names { Empty, Pistol, Grenade, StickyNade, Molotov, SniperRifle, AutoRifle, RailGun, GrenadeLauncher , BigPunch}
    public Names name = Names.Empty;

    //How many times an instance of this ability can be used before it vanished (default 0)
    public int Uses = 0;

    //The cooldown between activations (default 0)
    public float Cooldown = 0;

    //How this ability is used eg hold for a flamethrower, Press for a pistol  (Empty is purely for the empty class)
    public enum ActivationTypes { Empty, Press, Auto, Charge }
    public ActivationTypes ActivationType = ActivationTypes.Empty;

    //An empty function that is replaced with whatever the unique abilities do
    public abstract void Activate();
}

//A subclass/ child class that has scripts that are used for any ability that uses a projectile 
public class ProjectileWeapon : AbilityType
{
    //A referrence to the prefab of the projectile that will be shot
    public GameObject Projectile;

    //The activate function is replacing the other one, the referrenced projectile is instantiated at the position and rotation of the camera
    public override void Activate()
    {
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
            bullet = GameObject.Instantiate(Projectile, Camera.main.transform.position, Camera.main.transform.rotation);
            if (bullet.GetComponentInChildren<Rigidbody>().useGravity) {bullet.GetComponentInChildren<Rigidbody>().linearVelocity += Player.GetComponent<Rigidbody>().linearVelocity;}
        }
        //if theres a deactivated shot it is reset and appears to have just been created
        else
        {
            bullet.transform.position = Camera.main.transform.position;
            bullet.transform.rotation = Camera.main.transform.rotation;
            bullet.transform.GetChild(0).gameObject.SetActive(true);
            bullet.transform.GetChild(0).localPosition = Vector3.zero;
            bullet.transform.GetChild(0).localRotation = Projectile.transform.GetChild(0).localRotation;
            bullet.transform.GetChild(0).gameObject.GetComponent<Collider>().excludeLayers = LayerMask.GetMask("Items", "Player");
            if (bullet.GetComponentInChildren<Rigidbody>().useGravity) { bullet.GetComponentInChildren<Rigidbody>().linearVelocity += Player.GetComponent<Rigidbody>().linearVelocity; }
        }
    }

    //A constructor for projectile abilities
    public ProjectileWeapon(Names Name, GameObject projectile, Sprite icon, float cooldown = 0)
    {
        //the name of the ability
        name = Name;

        //a referrence to the player
        Player = GameObject.FindWithTag("Player");

        //A refference to the prefab
        Projectile = projectile;

        //Setting a reference to the correct icon sprite
        Icon = icon; 

        //setting the cooldown to the inputted value (by default 0)
        Cooldown = cooldown;   

        //Assigning unique values depending on the name
        switch (Name)
        {
            //if the ability is the pistol...
            case Names.Pistol:
            {
                    Uses = 3;
                    ActivationType = ActivationTypes.Press;
                    Cooldown = 1;
                    break;
            }

            //if the ability is the Grenade, or Sticky Grenade or Molotov...
            case Names.Grenade:
            case Names.StickyNade:
            case Names.Molotov:
                {
                    Uses = 1;
                    ActivationType = ActivationTypes.Press;
                    break;
                }

            //if the ability is the Grenade launcher...
            case Names.GrenadeLauncher:
                {
                    Uses = 3;
                    ActivationType = ActivationTypes.Press;
                    Cooldown = 0.1f;
                    break;
                }
        }
    }
}

//Empty ability, only use for emptying an ability instance
public class Empty : AbilityType
{
    public Empty()
    {
        name = Names.Empty;
        Uses = 0;
        ActivationType = ActivationTypes.Empty;
    }

    public override void Activate() { }
}

//A subclass of the ability type class which creates a raycast and does an action given by a subclass to the objects hit by the raycast
public abstract class RaycastAbility : AbilityType
{
    //the max length of the raycast
    public float MaxLength;

    //Creates a raycast then interacts with that which is hit
    public override void Activate()
    {
        RaycastHit[] Hits = Physics.RaycastAll(Player.transform.position, Camera.main.transform.forward, MaxLength, LayerMask.GetMask("Terrain", "Enemies"));
        DoWithRaycast(Hits);
    }

    //Empty function that interacts with the objects
    public abstract void DoWithRaycast(RaycastHit[] Hits);
}

//A further subclass used for all simple weapons that shoot a hitscan shot and deal an amount of damage
public class RayCastGun : RaycastAbility
{
    //The amount of damage a shot deals
    public float Damage;

    //Constructor
    public RayCastGun(Names Name, Sprite icon)
    {
        //the name of the ability
        name = Name;

        //a referrence to the player
        Player = GameObject.FindWithTag("Player");
        
        //setting the icon
        Icon = icon;

        //Assigning unique values depending on the name
        switch (Name)
        {
            //if the ability is the Sniper rifle...
            case Names.SniperRifle:
                {
                    Uses = 1;
                    ActivationType = ActivationTypes.Press;
                    Damage = 8;
                    MaxLength = 20;
                    break;
                }
            //if the ability is the automatic rifle...
            case Names.AutoRifle:
                {
                    Uses = 3;
                    ActivationType = ActivationTypes.Auto;
                    Damage = 4;
                    MaxLength = 20;
                    Cooldown = 1f;
                    break;
                }
        }
    }

    //takes the first object hit by the raycast and deals it damage equal to the parameter
    public override void DoWithRaycast(RaycastHit[] Hits)
    {
        if(Hits.Length > 0)
        {
            Hits[0].collider.gameObject.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver);
        }
    }
}

//Subclass for the railgun ability from the raycast ability subtype
public class RailGun : RaycastAbility
{
    //The default damage dealt by this
    public float Damage = 8;

    //Constructor
    public RailGun(Sprite icon)
    {
        //Setting the name
        name = Names.RailGun;

        //Setting the icon
        Icon = icon;

        //Fetching a reference to the player
        Player = GameObject.FindWithTag("Player");

        //Setting the amount of uses
        Uses = 1;

        //Setting the activation type
        ActivationType = ActivationTypes.Charge;

        //Setting the length of the raycast
        MaxLength = 30;
    }

    //Dealing damage
    public override void DoWithRaycast(RaycastHit[] Hits)
    {
        //Dealing damage to every object hit by the raycast until it hits terrain
        bool HasHitTerrain = false;
        int HitIndex = 0;
        while(!HasHitTerrain && HitIndex < Hits.Length)
        {
            if (Hits[HitIndex].collider.gameObject.tag == "Terrain")
            {
                HasHitTerrain = true;
            }
            else
            {
                Hits[HitIndex].collider.gameObject.SendMessage("TakeDamage", Mathf.Pow( Damage, Player.GetComponent<AbilityHolder>().CurrentCharge) , SendMessageOptions.DontRequireReceiver);
            }

            HitIndex++; 
        }

        //Resetting the charge
        Player.GetComponent<AbilityHolder>().CurrentCharge = 0;
    }
}

//The subclass for the big punch ability
public class BigPunch : AbilityType
{
    //What to do when this is activated
    public override async void Activate()
    {
        //Fetching a reference to the player's rigidbody
        Rigidbody rb = Player.GetComponent<Rigidbody>();

        //Flinging the player forward
        rb.AddForce(Player.transform.GetChild(0).forward * 500);

        //Dealing damage to every object hit by the raycast until it hits terrain
        RaycastHit[] Hits = Physics.RaycastAll(Player.transform.position, Camera.main.transform.forward, 2, LayerMask.GetMask("Terrain", "Enemies"));
        bool HasHitTerrain = false;
        int HitIndex = 0;
        while (!HasHitTerrain && HitIndex < Hits.Length)
        {
            if (Hits[HitIndex].collider.gameObject.tag == "Terrain")
            {
                HasHitTerrain = true;
            }
            else
            {
                Hits[HitIndex].collider.gameObject.SendMessage("TakeDamage", 10, SendMessageOptions.DontRequireReceiver);
            }

            HitIndex++;
        }

        //Waiting 200 ms
        await Task.Delay(200);

        ///Dealing damage to every object hit by a raycast until it hits terrain (again)
        Hits = Physics.RaycastAll(Player.transform.position, Camera.main.transform.forward, 2, LayerMask.GetMask("Terrain", "Enemies"));
        HasHitTerrain = false;
        HitIndex = 0;
        while (!HasHitTerrain && HitIndex < Hits.Length)
        {
            if (Hits[HitIndex].collider.gameObject.tag == "Terrain")
            {
                HasHitTerrain = true;
            }
            else
            {
                Hits[HitIndex].collider.gameObject.SendMessage("TakeDamage", 10, SendMessageOptions.DontRequireReceiver);
            }

            HitIndex++;
        }

        //flinging the player up
        if (rb.linearVelocity.y < 0) { rb.linearVelocity = Vector3.Scale(rb.linearVelocity, new Vector3(1, 0.3f, 1)); }
        rb.AddForce(Vector3.up * 350);
    }

    //Constructor
    public BigPunch(Sprite icon)
    {
        //Setting the icon
        Icon = icon;

        //fetching a reference to the player
        Player = GameObject.FindWithTag("Player");

        //Setting the name
        name = Names.BigPunch;

        //Setting the number of uses
        Uses = 1;

        //Setting the activation type
        ActivationType = ActivationTypes.Press;
    }
}

//This is stored in the player's ability slots
class AbilityInstance
{
    //The image representing the ability
    public Sprite Icon;

    //the type of ability it's storing
    public AbilityType abilityType;

    //the amount of uses remaining
    public int currentUses = 0;

    //basic constructor
    public AbilityInstance(AbilityType Types)
    {
        abilityType = Types;
        Icon = abilityType.Icon;    
        currentUses = Types.Uses;
    }

    //reduces the uses of the ability and uses the ability
    public void Activate()
    {
        abilityType.Activate();
        currentUses--;

        if (currentUses == 0) { Empty(); }
    }

    //"Empties" the slot
    public void Empty()
    {
        Icon = null;
        abilityType = new Empty();
    }
}

