using UnityEngine;

public class DoorautoOpen : MonoBehaviour
{

    GameObject Player;
    public Vector3 playerprevpos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        playerprevpos = Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerprevpos != Player.transform.position) {gameObject.SetActive(false);}
    }
}
