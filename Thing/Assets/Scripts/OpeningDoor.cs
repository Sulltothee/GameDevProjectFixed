using System.Collections;
using TMPro;
using UnityEngine;

public class OpeningDoor : MonoBehaviour
{ 
    //The final elevation above the initial position
    public float MaxElevation;

    //How much the elevation increases every second
    public float DistancePersec;

    //called by other objects to start the opening of the door
    public void StartOpen()
    {
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
}
