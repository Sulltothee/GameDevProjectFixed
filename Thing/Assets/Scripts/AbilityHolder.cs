using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    //All the possible Types of abilities
    AbilityType[] AbilityTypes;

    //The currently Held abilities
    AbilityInstance[] CurrentAbilities;

    // Start is called before the first frame update
    void Start()
    {
        CurrentAbilities = new AbilityInstance[4];
        for (int i = 0; i < AbilityTypes.Length; i++)
        {
            CurrentAbilities[i].Empty();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}

public class AbilityType
{
    public enum Names { Empty }
    Names name = Names.Empty;

    public int Uses = 0;

    public enum ActivationTypes { Empty, Press, Hold }
    ActivationTypes ActivationType = ActivationTypes.Empty;

    public virtual void Activate() { }

    public AbilityType()
    {
        name = Names.Empty;
        Uses = 0;
        ActivationType = ActivationTypes.Empty;
    }
}

public class ProjectileWeapon : AbilityType
{
    public GameObject Projectile;

    public override void Activate()
    {
        GameObject.Instantiate(Projectile);
    }
}

class AbilityInstance
{
    public Sprite Icon;

    AbilityType abilityType;

    public int currentUses = 0;

    public AbilityInstance(AbilityType Types)
    {
        abilityType = Types;

        currentUses = Types.Uses;
    }

    public void Activate()
    {
        abilityType.Activate();
        currentUses--;

        if (currentUses == 0) { Empty(); }
    }

    public void Empty()
    {
        Icon = null;
        abilityType = new AbilityType();
    }
}

