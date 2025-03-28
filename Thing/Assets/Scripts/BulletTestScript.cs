using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTestScript : MonoBehaviour
{
    //the magnitude of velocity the bullet travels at
    public float BulletSpeed = 100;
    public float Damage = 8;

    // Start is called before the first frame update
    private void Start()
    {
        //When first created it sets its own velocity
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);
    }

    private void OnEnable()
    {
        //When enabled it sets its own velocity
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the bullet hits an object that isn't the player it is "destroyed" (deactivated)
        
         gameObject.SetActive(false); other.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
