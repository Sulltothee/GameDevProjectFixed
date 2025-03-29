using UnityEngine;

public class Resta : MonoBehaviour
{
    //The starting position of the player
    Vector3 PlayerInitialPos;
    private void Start()
    {
        //Getting the player's position when the scene is loaded
        PlayerInitialPos = GameObject.FindWithTag("Player").transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if this collides with the player they are sent to the start position
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.position = PlayerInitialPos;
        }
    }
}
