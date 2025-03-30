using UnityEngine;

public class SniperDrone : DroneScript
{
    //How much damage a hit deals
    public float Damage = 1;

    //The last position of the player, allows them to "dodge" the shot
    Vector3 LastKnownPlayerPosition = Vector3.zero;

    //The renderer component for the laser
    public LineRenderer lineRenderer;

    //How long between shots/ how long until the next shot
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;


    //How far it can shoot you from
    public float Range;

    //What it does when enabled
    public override void SubFreshStart()
    {
        //fetching and enabling the line renderer component
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
    }

    public override void SubFixedUpdate()
    {
        //Decreasing the cooldown for the next attack
        if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }

        //Attacking
        else if (currentState != states.death && (player.transform.position - transform.position).magnitude <= Range && Physics.Raycast(transform.position, player.transform.position - transform.position, Range) && AttackCooldownCurrent <= 0)
        {
            //Changing the state
            currentState = states.shoot;

            //Restarting the cooldown
            AttackCooldownCurrent = AttackCooldown;

            //Setting the lastknown player position so it can miss the shot
            LastKnownPlayerPosition = player.transform.position;

            //Shooting the shot after 0.5 seconds have passed
            Invoke("ShootGun", 0.5f);
        }

        //Laser stuff
        //Changing the last known player position if the enemy isn't shooting so you can see where it's aiming towards
        if (currentState != states.shoot) { LastKnownPlayerPosition = player.transform.position; }

        //Determining the laser's end point
        RaycastHit[] allHits = Physics.RaycastAll(transform.position, LastKnownPlayerPosition - transform.position, Range, LayerMask.GetMask("Terrain", "Player"));
        Vector3 point2;
        if (allHits.Length > 0) { point2 = allHits[0].point; }
        else { point2 = transform.position + ((LastKnownPlayerPosition - transform.position).normalized * Range); }

        //Setting the start and end points of the laser
        if (lineRenderer != null && lineRenderer.enabled)
        {
            lineRenderer.SetPositions(new Vector3[] { transform.position, point2 });
        }
    }

    //Called to shoot the gun
    void ShootGun()
    {
        //disabling the movement (temporarily)
        DoMovement = false; rb.linearVelocity = Vector3.zero;

        //Dealing damage to the player if they are within the laser
        RaycastHit[] hits = Physics.RaycastAll(transform.position, LastKnownPlayerPosition - transform.position, Range, LayerMask.GetMask("Terrain", "Player"));
        if (hits.Length > 0 && hits[0].collider.gameObject.tag == "Player")
        {
            player.SendMessage("TakeDamage", Damage);
        }

        //enabling the pathfinding after a delay
        Invoke("enablepathfinding", 1f);
    }

    public override void SubDying()
    {
        lineRenderer.enabled = false;
    }
}
