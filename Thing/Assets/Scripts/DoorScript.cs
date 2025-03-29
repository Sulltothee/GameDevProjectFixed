using UnityEngine;

public class DoorScript : MonoBehaviour
{
    //The maximum and current hp of this door
    public float HPMax;
    public float HPCurrent;

    //this is called by other objects to deal this damage
    public void TakeDamage(float damage)
    {
        HPCurrent -= damage;
        if (HPCurrent <= 0 ) { gameObject.SetActive(false); }
    }
}
