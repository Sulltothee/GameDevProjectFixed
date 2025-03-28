using Unity.VisualScripting;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float Damage;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver); ;
    }
}
