using UnityEngine;
using UnityEngine.AI;

public class ChargerScript : EnemyBase
{
    //The current behavioral state of it
    public enum States { Walk, Charge, Idle, Death}
    public States CurrentState = States.Idle;

    //The rigidbody of this enemy
    Rigidbody rb;

    //the navmesh agent component
    public NavMeshAgent Agent;

    //How far away it can see you
    public float SeeDistance = 100;

    //How close it has to be to the player to charge;
    public float ChargeDistance = 10;

    //How fast it charges
    public float ChargeSpeed = 10;

    //What direction it's charging in
    Vector3 ChargeDirection;

    //How tall this enemy is
    public float height = 4;

    //How long this will charge before giving up
    public float ChargeTime = 10;

    //Whether this can charge
    public bool CanCharge = false;

    //How much time has to pass between one charge ending and another beginning
    public float ChargeInterval = 3;

    //How much force is applied to the player when they collide with a charge in the x and z/ y axis
    public float PushStrengthFlat = 1;
    public float PushStrengthY = 1;

    //How much damage this deals when colliding with it during a charge
    public float Damage = 1;

    public override void SubFreshStart()
    {
        rb = GetComponent<Rigidbody>();

        Agent = GetComponent<NavMeshAgent>();

        CurrentState = States.Idle;

        ChargeDirection = Vector3.zero;

        CanCharge = false;

        Invoke("SetChargeable", ChargeInterval);
    }

   public override void SubFixedUpdate()
    {
        //Determining whether it can see the player
        bool CanSeePlayer = !(Physics.Raycast(transform.position, player.transform.position - transform.position, (player.transform.position - transform.position).magnitude, LayerMask.GetMask("Terrain")) || (player.transform.position - transform.position).magnitude > SeeDistance);

        //Enabling it if it can see the player
        if (CanSeePlayer && CurrentState == States.Idle)
        { CurrentState = States.Walk; }

        //Making it walk towards the player
        if(Agent != null && Agent.enabled && Agent.isOnNavMesh && CurrentState == States.Walk)
        {
            Agent.destination = player.transform.position;
        }

        //Charging if it sees and is close enough to the player
        if(CanSeePlayer && Vector3.Scale((player.transform.position - transform.position), new Vector3(1,0,1)).magnitude < ChargeDistance && CurrentState == States.Walk && CanCharge)
        {
            Agent.enabled = false;
            CanCharge = false;
            
            Invoke("StartCharge", 2f);
        }
        if (CurrentState == States.Charge)
        { rb.linearVelocity = ChargeDirection * ChargeSpeed + Vector3.Scale(rb.linearVelocity, Vector3.up); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Terrain") && collision.GetContact(0).point.y > transform.position.y - height/2 && CurrentState == States.Charge)
        {
            CanCharge = false;
            CurrentState = States.Walk;
            rb.linearVelocity = Vector3.zero;

            Agent.enabled = true;

            Invoke("SetChargeable", ChargeInterval);
        }
        else if (collision.gameObject.CompareTag("Player") && CurrentState == States.Charge)
        {
            player.GetComponent<Rigidbody>().AddForce((Vector3.Scale((player.transform.position - transform.position), new Vector3(1, 0, 1)).normalized * PushStrengthFlat) + (Vector3.up * PushStrengthY));
            player.SendMessage("TakeDamage", Damage);
        }
    }

    void StartCharge()
    {
        ChargeDirection = Vector3.Scale((player.transform.position - transform.position), new Vector3(1, 0, 1)).normalized;
        CurrentState = States.Charge;
        Invoke("StopCharge", ChargeTime);
    }

    void StopCharge()
    {
        if (CurrentState == States.Charge)
        {
            CanCharge = false;
            CurrentState = States.Walk;
            rb.linearVelocity = Vector3.zero;
            if (Agent.isOnNavMesh)
            {
                Agent.enabled = true;
            }
            Invoke("SetChargeable", ChargeInterval);
        }
    }

    void SetChargeable()
    {
        CanCharge = true;
    }

    public override void DoOnDeath()
    {
        Agent.enabled = false;
        CurrentState = States.Death;
        CanCharge = false;
    }
}
