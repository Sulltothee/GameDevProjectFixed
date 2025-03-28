using UnityEngine;

public class Billboarder : MonoBehaviour
{


   
    public GameObject Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.forward = Vector3.Scale(Player.transform.position - transform.position, new Vector3(1, 0.1f, 1));
    }
}
