using UnityEngine;

public class AbilityItem : MonoBehaviour
{
    //What type of ability this will give the player upon collision
    public AbilityType.Names Type;

    //whether this can be picked up multiple times or it dissapears after one pick up
    public bool DestroyOnCollide = true;

    //the player object
    GameObject Player;

    //the icons for the different ability types
    Sprite[] Icons;

    //The sprite renderer component
    SpriteRenderer SpriteRend;

    //How far up and down this can go when hovering (can go up by this amount and down by this amount)
    public float wiggleDistance;

    //How fast this hovers up and down/ where in the sine is the current level of hover
    public float WiggleSpeed; 
    float CurrentWiggle;

    //The initial position of this object
    float Defaulty;

    private void Awake()
    {
        //Fetching a reference to the player
        Player = GameObject.FindWithTag("Player");

        //Fetching the list of all the ability icons
        Icons = Player.GetComponent<AbilityHolder>().Icons;

        //fetching the sprite renderer component
        SpriteRend = GetComponent<SpriteRenderer>();
        
        //Aquiring the default position
        Defaulty = transform.position.y;

        //resetting this objects position in the sine wave
        CurrentWiggle = 0;
    }

    public void Resety(float Y)
    {
        Defaulty = Y;
    }

    private void OnTriggerEnter(Collider other)
    {
        //When it collides with the player
        if (other.gameObject.tag == "Player")
        {
            //Attempts to give the player the ability, if successful (the player's inventory wasn't full) and this destroys itself upon delivering the item, it disables itself
            if (other.GetComponent<AbilityHolder>().GiveAbility(Type) && DestroyOnCollide) { gameObject.SetActive(false); }
        }
    }

    private void FixedUpdate()
    {
        //Billboarding 
        transform.forward = Vector3.Scale(Player.transform.position - transform.position, new Vector3(1, 0.1f, 1));
        SpriteRend.sprite = Icons[(int)Type - 1];

        //moving this item's height with the sine wave to make it hover/wiggle
        transform.position = new Vector3( transform.position.x, Defaulty + (wiggleDistance * Mathf.Sin(CurrentWiggle)), transform.position.z);
        CurrentWiggle += WiggleSpeed * 0.02f;
    }
}
