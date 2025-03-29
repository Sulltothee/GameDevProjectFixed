using UnityEngine;

public class MolotovExplosion : MonoBehaviour
{
    //How much burn time is applied when an object enters/ stays in this hitbox
    public float BurnTimeAppliedPerSec;

    //Adding burn to any objects that enter or stay in this hitbox
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SendMessage("ApplyBurn", BurnTimeAppliedPerSec * 0.02f, SendMessageOptions.DontRequireReceiver);
    }
    private void OnTriggerStay(Collider other)
    {
        other.gameObject.SendMessage("ApplyBurn", BurnTimeAppliedPerSec * 0.02f, SendMessageOptions.DontRequireReceiver);
    }
}
