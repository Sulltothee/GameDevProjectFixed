using UnityEngine;

public class DeathPit : MonoBehaviour
{
    //When something enters this collider it is dealt enough damage to kill it
    private void OnTriggerEnter(Collider other)
    {
        other.SendMessage("TakeDamage", 1000, SendMessageOptions.DontRequireReceiver);
    }
}
