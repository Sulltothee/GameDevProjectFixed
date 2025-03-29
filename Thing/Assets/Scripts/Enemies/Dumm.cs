using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dumm : MonoBehaviour
{
    //The current hp
    public float HP;

    //the type of ability that will be dropped/ the ability item prefab
    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    //how long this object will burn for/ how much damage it will take for each second it burns
    float Burntime;
    public float BurnDmgPerSec;

    //Called by other scripts to deal this damage
    public void TakeDamage(float damage)
    {
        HP -= damage;
    }

    //called by other objects to "ignite" this object
    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }

    private void FixedUpdate()
    {
        //dying
        if(HP <= 0) 
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
            if (!HasFoundUsable ) 
            {
                 GameObject currentObject =  GameObject.Instantiate(AbilityDrop, transform.position, Quaternion.identity);
                 currentObject.transform.GetChild(0).GetComponent<AbilityItem>().Type = Name;
            }
            gameObject.SetActive(false);
        }

        //applying burn damage
        if (Burntime > 0)
        {
            HP -= BurnDmgPerSec * 0.02f;
            Burntime -= 0.02f;
        }
    }
}
