using UnityEngine;

public class MolotovScript : MonoBehaviour
{
    //the magnitude of velocity the grenade travels at
    public float BulletSpeed = 0;

    //the explosion object
    GameObject Explosion;

    private void Awake()
    {
        //when this object is "created" it adds it's own velocity.
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);

        //Fetches a reference to the explosion hit box
        Explosion = transform.GetChild(0).gameObject;

        //reenables gravity
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnEnable()
    {
        //when this object is "created" it adds it's own velocity.
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);

        //Fetches a reference to the explosion hit box
        Explosion = transform.GetChild(0).gameObject;

        //reenables gravity
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Enables the explosion hitbox
        Explosion.SetActive(true);

        //Stops itself from moving
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = false;

        //Disables the explosion hitbox and itself after a timer
        Invoke("DisableExplosion", 3f);

    }

    //Disables the explosion hitbox and itself after a timer
    void DisableExplosion()
    {
        Explosion.SetActive(false);
        gameObject.SetActive(false);
    }
}
