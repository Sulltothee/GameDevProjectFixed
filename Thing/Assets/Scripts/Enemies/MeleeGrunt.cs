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

    public override void SubFreshStart()
    {
        //fetches the navmeshagent component
        Agent = GetComponent<NavMeshAgent>();
    }
    private void FixedUpdate()
    {
        //creating a path to the player
        if (Agent != null) 
        {
            Agent.destination = player.transform.position;
        }

        //Attacking
        if (AttackCooldownCurrent > 0) { AttackCooldownCurrent -= 0.02f; }
        else if((player.transform.position - transform.position).magnitude <= Range && AttackCooldownCurrent <= 0)
        {
            player.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver);
            AttackCooldownCurrent = AttackCooldown;
        }
    }
}
