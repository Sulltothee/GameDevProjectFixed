using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RangeGrunt : EnemyBase
{
    //The current behavioral state of it
    public enum states { idle,run,death,shoot}
    public states currentState = states.run;

    //The animator component
    public Animator animator;

    //The navmeshagent component
    NavMeshAgent Agent;

    //How far it can attack from/ the projectile it fires when it shoots
    public float Range;
    public GameObject Projectile;

    //How often it attacks/ how long until the next attack
    public float AttackCooldown = 3;
    public float AttackCooldownCurrent = 0;

    public override void SubFreshStart()
    {
        //fetches the navmeshagent and animator components 
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        AttackCooldownCurrent = AttackCooldown;
    }

    public override void SubFixedUpdate()
    {
        //creating a new path towards the player
        if (Agent != null && Agent.enabled) 
        {
            Agent.destination = player.transform.position;
        }

        //Deincrementing the cooldown if it has yet to pass
        if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }
        //shooting the if the cooldown has passed, the enemy isn't currently dying and the player is within range and visible
        else if (currentState != states.death && (player.transform.position - transform.position).magnitude <= Range && !Physics.Raycast(transform.position, player.transform.position - transform.position,Range, LayerMask.GetMask("Terrain")) && AttackCooldownCurrent <= 0)
        {
            //changing the state
            currentState = states.shoot;

            //restarting the cooldown
            AttackCooldownCurrent = AttackCooldown;

            //Shooting the projectile after sufficient animation
            Invoke("ShootGun", 0.5f);
        }

        //updating the animations in line with the current state
        if (animator != null) { animator.SetInteger("StateInt", (int)currentState); }
    }

           
    //enables the pathfinding after it has shot
    void enablepathfinding() 
    {
        Agent.enabled = true;
        currentState = states.run;
    }

    void ShootGun()
    {
        //disables the pathfinding until it shoots
        Agent.enabled = false;

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
            bullet = GameObject.Instantiate(Projectile, transform.position, transform.rotation);
        }
        //if theres a deactivated shot it is reset and appears to have just been created
        bullet.transform.position = transform.position;
        bullet.transform.forward = player.transform.position - transform.position;
        bullet.transform.GetChild(0).gameObject.SetActive(true);
        bullet.transform.GetChild(0).localPosition = Vector3.zero;
        bullet.transform.GetChild(0).localRotation = Projectile.transform.GetChild(0).localRotation;
        bullet.transform.GetChild(0).gameObject.GetComponent<Collider>().excludeLayers = LayerMask.GetMask("Items", "Enemies");

        //reenables the pathfinding
        Invoke("enablepathfinding", 1f);
    }

    public override void DoOnDeath()
    {
        currentState = states.death;
        Agent.enabled = false;
    }
}
