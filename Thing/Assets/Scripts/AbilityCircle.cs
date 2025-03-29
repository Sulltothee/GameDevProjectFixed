using UnityEngine;

public class AbilityCircle : MonoBehaviour
{
    //a reference to the current ability info of the player
    AbilityHolder player;

    //the transform (position) of this object (it's ui so needs to be referenced)
    RectTransform Transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //fetching a reference to the ability info
        player = GameObject.FindWithTag("Player").GetComponent<AbilityHolder>();

        //fetching a reference to this object's transform
        Transform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //updating this object's position to be around the currently selected ability slot
        Transform.localPosition = new Vector3( -450 + (player.AbilityIndex * 300), -300);
    }
}
