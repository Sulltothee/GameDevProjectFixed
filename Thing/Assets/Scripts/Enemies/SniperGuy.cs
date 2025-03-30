using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SniperGuy : EnemyBase
{
    //State machine for animations
    public enum states { idle,run,death,shoot}
    public states currentState = states.idle;

    //the animator component
    Animator animator;

    //The navmesh agent component used for navigation
    NavMeshAgent Agent;

    //How far it can shoot you from
    public float Range;

    //How long between shots/ how long until the next shot
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    //How much damage a hit deals
    public float Damage = 1;

    //The last position of the player, allows them to "dodge" the shot
    Vector3 LastKnownPlayerPosition = Vector3.zero;

    //The renderer component for the laser
    LineRenderer lineRenderer;

    public override void SubFreshStart()
    {
        //fetches the navmeshagent, animator and line renderer components
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        lineRenderer = GetComponent<LineRenderer>();

        //Reenabling the laser
        lineRenderer.enabled = true;

        //enabling the pathfinding
        Agent.enabled = true;
    }

    public override void SubFixedUpdate()
    {
        //If the navmeshagent isn't disabled, make a path towards the player
        if (Agent != null && Agent.enabled)
        {
            Agent.destination = player.transform.position;
        }

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

        //applying the current state to the animator
        if (animator != null) { animator.SetInteger("StateInt", (int)currentState); }

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

    //Called after a delay to allow the object to wander after stopping to shoot
    void enablepathfinding()
    {
        if (currentState != states.death)
        {
            Agent.enabled = true;
            currentState = states.run;
        }
    }

    //Called to shoot the gun
    void ShootGun()
    {
        //disabling the movement (temporarily)
        Agent.enabled = false;

        //Dealing damage to the player if they are within the laser
        RaycastHit[] hits = Physics.RaycastAll(transform.position, LastKnownPlayerPosition - transform.position, Range, LayerMask.GetMask("Terrain", "Player"));
        if (hits.Length > 0 && hits[0].collider.gameObject.tag == "Player")
        {
            player.SendMessage("TakeDamage", Damage);
        }

        //enabling the pathfinding after a delay
        Invoke("enablepathfinding", 1f);
    }

    public override void DoOnDeath()
    {
        currentState = states.death;
        Agent.enabled = false;
        lineRenderer.enabled = false;
    }
}
