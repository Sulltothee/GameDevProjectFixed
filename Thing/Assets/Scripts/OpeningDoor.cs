using System.Collections;
using TMPro;
using UnityEngine;

public class OpeningDoor : MonoBehaviour
{ 
    //The final elevation above the initial position
    public float MaxElevation;

    //How much the elevation increases every second
    public float DistancePersec;

    //All the enemies that must be dead for it to open
    public GameObject[] enemies;

    //Whether the door is currently opening
    bool isOpening = false;

    //called by other objects to start the opening of the door
    public void StartOpen()
    {
        isOpening = true;
        StartCoroutine(Raise());
    }

    //Every frame after the door has started to open it's elevation increases by some amount
    IEnumerator Raise()
    {
        for (float CurrentElevation = 0; CurrentElevation <= MaxElevation ; CurrentElevation += Time.deltaTime * DistancePersec)
        {
            transform.position += new Vector3(0, Time.deltaTime * DistancePersec, 0);
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if (!isOpening)
        {
            bool CanOpen = true;
            foreach (GameObject enemy in enemies)
            {
                if (enemy.transform.GetChild(0).gameObject.activeSelf && CanOpen)
                {
                    CanOpen = false;
                }
            }
            if (CanOpen) { StartOpen(); }
        }
    }
}
