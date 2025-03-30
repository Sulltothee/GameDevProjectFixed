using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class BigMelee : EnemyBase
{
    //The current behavioral state of it
    public enum states { idle, run, death, Punch }
    public states currentState = states.idle;

    //How close you have to be to be "seen"
    public float SeeDistance;

    //The animator component
    Animator animator;

    //The navmeshagent component
    NavMeshAgent Agent;

    //How far it can attack from and how much damage it deals
    public float Range = 6;
    public float Damage;

    //How often it attacks/ how long until the next attack
    public float AttackCooldown = 3;
    float AttackCooldownCurrent = 0;

    //How long it takes for it to attack
    public float PunchTime;

    public override void SubFreshStart()
    {
        //fetches the navmeshagent and animator components 
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }


    // fixed update is called every 0.02s
    public override void SubFixedUpdate()
    {
        //Enabling/ disabling its behaviors if it can't see the player or if it's dying
        if(currentState != states.death && currentState!= states.Punch && !Physics.Raycast(transform.position, player.transform.position - transform.position, SeeDistance, -player.layer )) {currentState = states.idle;} 
        else if (currentState != states.death && currentState != states.Punch) { currentState = states.run;}
        if (Agent != null && Agent.enabled && currentState == states.run)
        {
            Agent.destination = player.transform.position;
        }

        //Attacking
        if(AttackCooldownCurrent <= 0 && (player.transform.position - transform.position).magnitude <= Range) 
        {
            //restarting the cooldown
            AttackCooldownCurrent = AttackCooldown;

            //changing the state
            currentState = states.Punch;

            //finishing the punch after the animation has played some and the player has had a chance to run
            Invoke("FinishPunch", PunchTime);
        }
        //decreasing the attack cooldown if it hasn't passed
        else if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }

        //Updating the animations in reference to the current state
        animator.SetInteger("StateInt", (int)currentState);
    }

    public override async void DoOnDeath()
    {
        currentState = states.death;
        Agent.enabled = false;

        await Task.Delay(1000);
    }

    //called to reenable the pathfinding and deal damage to the player at the end of a punch
    public void FinishPunch()
    {
        //Deals damage to the player if they're still in range
        if ((player.transform.position - transform.position).magnitude <= Range)
        {
            player.SendMessage("TakeDamage", Damage);
        }
        currentState = states.run;
    }
}

