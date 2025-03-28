using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public float HPMax;
    public float HPCurrent;

    public void TakeDamage(float damage)
    {
        HPCurrent -= damage;
        if (HPCurrent <= 0 ) { gameObject.SetActive(false); }
    }
}
