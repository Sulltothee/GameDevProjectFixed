using UnityEngine;

public class StickyNade : MonoBehaviour
{
    //the magnitude of velocity the bullet travels at
    public float BulletSpeed = 0;
    GameObject Explosion;
    public float AutoExplodeTime = 0;
    float CurrentAutoExplodeTime;
    bool HasExploded = false;

    GameObject StuckObject;
    bool HasStuckObject = false;
    Vector3 StuckVector;

    // Start is called before the first frame update
    private void Start()
    {
        //When first created it sets its own velocity
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);
        Explosion = transform.GetChild(0).gameObject;
        CurrentAutoExplodeTime = AutoExplodeTime;
        HasExploded = false;
    }

    private void OnEnable()
    {
        //When enabled it sets its own velocity
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);
        Explosion = transform.GetChild(0).gameObject;
        GetComponent<Rigidbody>().useGravity = true;
        CurrentAutoExplodeTime = AutoExplodeTime;
        HasExploded = false; HasStuckObject = false;
    }

    private void FixedUpdate()
    {
        if (CurrentAutoExplodeTime > 0 && !HasExploded)
        {
            CurrentAutoExplodeTime -= 0.2f;
            if (CurrentAutoExplodeTime <= 0)
            {
                HasExploded = true;
                Explosion.SetActive(true);
                Invoke("DisableExplosion", 0.5f);
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                GetComponent<Rigidbody>().useGravity = false;
            }
        }

        if (HasStuckObject && !HasExploded)
        {
            transform.position = StuckObject.transform.position - StuckVector;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!HasStuckObject && collision.gameObject.tag != "Player")
        {
            StuckObject = collision.gameObject;
            StuckVector = collision.transform.position - transform.position;
            HasStuckObject = true;
            GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void DisableExplosion()
    {
        Explosion.SetActive(false);
        gameObject.SetActive(false);
    }
}
