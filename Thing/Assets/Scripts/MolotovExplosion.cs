using UnityEngine;

public class MolotovExplosion : MonoBehaviour
{
    public float BurnTimeAppliedPerSec;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SendMessage("ApplyBurn", BurnTimeAppliedPerSec * 0.02f, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerStay(Collider other)
    {
        other.gameObject.SendMessage("ApplyBurn", BurnTimeAppliedPerSec * 0.02f, SendMessageOptions.DontRequireReceiver);
    }
}
