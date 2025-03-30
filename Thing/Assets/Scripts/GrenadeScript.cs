using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    //the magnitude of velocity the bullet travels at
    public float BulletSpeed = 0;

    //The explosion object that is created when the grenade explodes
    GameObject Explosion;

    //How long it has to exist for until it explodes/ how long until this explodes
    public float AutoExplodeTime = 0;
    float CurrentAutoExplodeTime;

    //if it has exploded or not
    bool HasExploded = false;

    private void Awake()
    {
        //When enabled it sets its own velocity
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);

        //fetching the (currently disabled) explosion object
        Explosion = transform.GetChild(0).gameObject;

        //reenabling gravity (it stops using it when it explodes)
        GetComponent<Rigidbody>().useGravity = true;

        //resetting the explode timer and the explosion status
        CurrentAutoExplodeTime = AutoExplodeTime;
        HasExploded = false;
    }

    private void OnEnable()
    {
        //When enabled it sets its own velocity
        gameObject.GetComponent<Rigidbody>().linearVelocity = (transform.up * BulletSpeed);

        //fetching the (currently disabled) explosion object
        Explosion = transform.GetChild(0).gameObject;

        //reenabling gravity (it stops using it when it explodes)
        GetComponent<Rigidbody>().useGravity = true;

        //resetting the explode timer and the explosion status
        CurrentAutoExplodeTime = AutoExplodeTime;
        HasExploded = false;
    }

    private void FixedUpdate()
    {
        //If it hasn't exploded yet
        if(CurrentAutoExplodeTime > 0 && !HasExploded) 
        { 
            //decreasing the explosion timer
            CurrentAutoExplodeTime -= 0.2f;

            //if the timer is up, it "explodes"
            if (CurrentAutoExplodeTime <= 0) 
            {
                //sets it to have exploded 
                HasExploded = true;

                //enables the explosion hitbox
                Explosion.SetActive(true);

                //Disables the explosion hitbox and itself after a timer
                Invoke("DisableExplosion", 0.5f);

                //Stops itself from moving
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }

    //Disables the explosion hitbox and itself after a timer
    private void DisableExplosion()
    {
        Explosion.SetActive(false);
        gameObject.SetActive(false);
    }
}
