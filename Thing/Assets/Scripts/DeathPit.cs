using UnityEngine;

public class DeathPit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.SendMessage("TakeDamage", 1000, SendMessageOptions.DontRequireReceiver);
    }
}
