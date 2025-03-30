using UnityEngine;

public class Billboarder : MonoBehaviour
{
    //A reference to the player object
    public GameObject Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Fetching a reference to the player
        Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //turning the object to face the player
        transform.forward = Vector3.Scale(Player.transform.position - transform.position, new Vector3(1, 0.1f, 1));
    }
}
