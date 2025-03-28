using UnityEngine;

public class AbilityItem : MonoBehaviour
{
    public AbilityType.Names Type;
    public bool DestroyOnCollide = true;
    GameObject Player;
    Sprite[] Icons;
    SpriteRenderer SpriteRend;

    public float wiggleDistance;
    public float WiggleSpeed; float CurrentWiggle;
    Vector3 DefaultPosition;

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player");
        Icons = Player.GetComponent<AbilityHolder>().Icons;
        SpriteRend = GetComponent<SpriteRenderer>();
        
        DefaultPosition = GetComponent<Transform>().position;
        CurrentWiggle = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.GetComponent<AbilityHolder>().GiveAbility(Type) && DestroyOnCollide) { gameObject.SetActive(false); }
        }
    }

    private void FixedUpdate()
    {
        transform.forward = Vector3.Scale(Player.transform.position - transform.position, new Vector3(1, 0.1f, 1));

        SpriteRend.sprite = Icons[(int)Type - 1];

        transform.position = new Vector3( DefaultPosition.x , DefaultPosition.y + (wiggleDistance * Mathf.Sin(CurrentWiggle)), transform.position.z);

        CurrentWiggle += WiggleSpeed * 0.02f;
    }
}
