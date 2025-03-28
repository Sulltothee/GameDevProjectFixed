using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dumm : MonoBehaviour
{
    public float HP;
    public AbilityType.Names Name;
    public GameObject AbilityDrop;

    float Burntime;
    public float BurnDmgPerSec;

    public void TakeDamage(float damage)
    {
        HP -= damage;
    }

    public void ApplyBurn(float Time)
    {
        Burntime += Time;
    }

    private void FixedUpdate()
    {
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

        if (Burntime > 0)
        {
            HP -= BurnDmgPerSec * 0.02f;
            Burntime -= 0.02f;
        }
    }
}
