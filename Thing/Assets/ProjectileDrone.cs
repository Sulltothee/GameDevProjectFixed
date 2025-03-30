using UnityEngine;

public class ProjectileDrone : DroneScript
{
    //How far it can attack from/ the projectile it fires when it shoots
    public float Range;
    public GameObject Projectile;

    //How often it attacks/ how long until the next attack
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    //How much recoil force is applied to the drone
    public float recoilStrength = 0;

    public override void SubFixedUpdate()
    {
        //Deincrementing the cooldown if it has yet to pass
        if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }
        //shooting the if the cooldown has passed, the enemy isn't currently dying and the player is within range and visible
        else if (currentState != states.death && (player.transform.position - transform.position).magnitude <= Range && !Physics.Raycast(transform.position, player.transform.position - transform.position, Range, LayerMask.GetMask("Terrain")) && AttackCooldownCurrent <= 0)
        {
            //changing the state
            currentState = states.shoot;

            //restarting the cooldown
            AttackCooldownCurrent = AttackCooldown;

            //Shooting the projectile after sufficient animation
            Invoke("ShootGun", 0.5f);
        }
    }

    void ShootGun()
    {
        //disables the pathfinding until it shoots
        DoMovement = false;
        rb.linearVelocity = Vector3.zero;

        //Looking through every gameobject with the right tag to check whether they're disabled
        GameObject[] PotentialProjectiles = GameObject.FindGameObjectsWithTag(Projectile.tag);
        GameObject bullet = Camera.main.gameObject; bool hasFoundBullet = false;
        foreach (var projectile in PotentialProjectiles)
        {
            if (!projectile.transform.GetChild(0).gameObject.activeInHierarchy && !hasFoundBullet)
            {
                bullet = projectile;
                hasFoundBullet = true;
            }
        }

        //if a deactivated shot isn't found a new one is made
        if (!hasFoundBullet)
        {
            bullet = GameObject.Instantiate(Projectile, transform.position, Camera.main.transform.rotation);
            bullet.transform.forward = player.transform.position - transform.position;
            bullet.transform.GetChild(0).gameObject.GetComponent<Collider>().excludeLayers = LayerMask.GetMask("Items", "Enemies");
        }
        //if theres a deactivated shot it is reset and appears to have just been created
        else
        {
            bullet.transform.position = transform.position;
            bullet.transform.forward = player.transform.position - transform.position;
            bullet.transform.GetChild(0).gameObject.SetActive(true);
            bullet.transform.GetChild(0).localPosition = Vector3.zero;
            bullet.transform.GetChild(0).localRotation = Projectile.transform.GetChild(0).localRotation;
            bullet.transform.GetChild(0).gameObject.GetComponent<Collider>().excludeLayers = LayerMask.GetMask("Items", "Enemies");
        }

        //Recoil force
        rb.AddForce((transform.position - player.transform.position).normalized * recoilStrength);

        //reenables the pathfinding
        Invoke("enablepathfinding", 1f);
    }
}
