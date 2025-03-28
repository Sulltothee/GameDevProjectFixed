using UnityEngine;

public class AbilityCircle : MonoBehaviour
{
    AbilityHolder player;
    RectTransform TRansform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<AbilityHolder>();
        TRansform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        TRansform.localPosition = new Vector3( -450 + (player.AbilityIndex * 300), -300);
    }
}
