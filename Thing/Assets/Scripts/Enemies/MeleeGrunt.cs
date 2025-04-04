using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MeleeGrunt : EnemyBase
{
    //The navmeshagent component
    NavMeshAgent Agent;

    //How far it can attack from and how much damage it deals
    public float Range;
    public float Damage;

    //How often it attacks
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    //The current behavioral state of it
    public enum states { idle, run, death, Punch }
    public states currentState = states.idle;

    //The animator component
    public Animator animator;

    //How close the player has to be to activate this enemy
    public float SeeDistance;

    public override void SubFreshStart()
    {
        //fetches the navmeshagent component
        Agent = GetComponent<NavMeshAgent>();

        Agent.enabled = true;

        animator = GetComponentInChildren<Animator>();
    }
    public override void SubFixedUpdate()
    {
        //Activating
        if(currentState == states.idle && SeeDistance >= (player.transform.position - transform.position).magnitude && !Physics.Raycast(transform.position, player.transform.position - transform.position, SeeDistance, LayerMask.GetMask("Terrain")))
        {
            currentState = states.run;
        }

        //creating a path to the player
        if (Agent != null && Agent.enabled && currentState == states.run) 
        {
            Agent.destination = player.transform.position;
        }

        //Attacking
        if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }
        else if((player.transform.position - transform.position).magnitude <= Range && AttackCooldownCurrent <= 0)
        {
            currentState = states.Punch;
            AttackCooldownCurrent = AttackCooldown;
            Invoke("FinishPunch", 1);
        }

        //Updating the animations in reference to the current state
        animator.SetInteger("StateInt", (int)currentState);
    }

    public void FinishPunch()
    {
        if (currentState != states.death)
        {
            currentState = states.run;
            if (Range >= (player.transform.position - transform.position).magnitude)
            {
                player.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public override void DoOnDeath()
    {
        currentState = states.death;
        Agent.enabled = false;
    }
}
