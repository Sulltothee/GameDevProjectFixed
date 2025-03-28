using UnityEngine;

public class Resta : MonoBehaviour
{
    Vector3 PlayerInitialPos;

    private void Start()
    {
        PlayerInitialPos = GameObject.FindWithTag("Player").transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.position = PlayerInitialPos;
        }
    }
}
