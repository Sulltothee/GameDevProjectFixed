using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    //How Many abilities have been implemented
    public int NumOfAbilities;

    //all the projectiles for the weapons
    public GameObject[] Projectiles;

    //All the possible Types of abilities
    AbilityType[] AbilityTypes;

    //This points to the index of the currently selected abilities
    int AbilityIndex = 0;   

    //The currently Held abilities
    AbilityInstance[] CurrentAbilities;

    // Start is called before the first frame update
    void Start()
    {
        //Instantializing the ability slots
        CurrentAbilities = new AbilityInstance[4];

        //Assigning all the abilities there info
        //All the projectile weapons
        AbilityTypes = new AbilityType[NumOfAbilities];
        AbilityTypes[0] = new ProjectileWeapon(AbilityType.Names.Pistol, Projectiles[0]);


        //Test Stuff
        CurrentAbilities[0] = new AbilityInstance(AbilityTypes[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            CurrentAbilities[AbilityIndex].Activate();
        }
    }


}

//This class holds the basic components necessary for every ability type
public abstract class AbilityType
{
    //The player object, this stops the need to frequently find the player which will make unnesescary load
    public GameObject Player;

    //The names of every ability type/ the name of the instance (default empty)
    public enum Names { Empty, Pistol }
    public Names name = Names.Empty;

    //How many times an instance of this ability can be used before it vanished (default 0)
    public int Uses = 0;

    //How this ability is used eg hold for a flamethrower, Press for a pistol  (Empty is purely for the empty class)
    public enum ActivationTypes { Empty, Press, Hold }
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
        GameObject.Instantiate(Projectile, Camera.main.transform.position, Camera.main.transform.rotation);   
    }

    //A constructor for projectile abilities
    public ProjectileWeapon(Names Name, GameObject projectile)
    {
        //the name of the ability
        name = Name;

        //a referrence to the player
        Player = GameObject.FindWithTag("Player");

        //A refference to the prefab
        Projectile = projectile;

        //Assigning unique values depending on the name
        switch (Name)
        {
            //if the ability is the pistol...
            case Names.Pistol:
            {
                    Uses = 3;
                    ActivationType = ActivationTypes.Press;
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
        RaycastHit[] Hits = Physics.RaycastAll(Player.transform.position, Camera.main.transform.forward, MaxLength);
        DoWithRaycast(Hits);
    }

    //Empty function that interacts with the objects
    public abstract void DoWithRaycast(RaycastHit[] Hits);
}

//This is stored in the player's ability slots
class AbilityInstance
{
    //The image representing the ability
    public Sprite Icon;

    //the type of ability it's storing
    AbilityType abilityType;

    //the amount of uses remaining
    public int currentUses = 0;

    //basic constructor
    public AbilityInstance(AbilityType Types)
    {
        abilityType = Types;

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

