using UnityEngine;

public class MolotovScript : MonoBehaviour
{

    //the magnitude of velocity the bullet travels at
    public float BulletSpeed = 0;
    GameObject Explosion;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);
        Explosion = transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);
        Explosion = transform.GetChild(0).gameObject;
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explosion.SetActive(true);
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = false;
        Invoke("DisableExplosion", 3f);

    }

    void DisableExplosion()
    {
        Explosion.SetActive(false);
        gameObject.SetActive(false);
    }
}
