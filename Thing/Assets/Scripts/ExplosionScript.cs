using Unity.VisualScripting;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    //How much damage this deals to any that enter the hitbox
    public float Damage;

    private void OnTriggerEnter(Collider other)
    {
        //Dealing any colliding object damage
        other.gameObject.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver); ;
    }
}
