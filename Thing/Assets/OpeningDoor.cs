using System.Collections;
using TMPro;
using UnityEngine;

public class OpeningDoor : MonoBehaviour
{ 
    public float MaxElevation;
    public float DistancePersec;

    public void StartOpen()
    {
        StartCoroutine(Raise());
    }

    IEnumerator Raise()
    {
        for (float CurrentElevation = 0; CurrentElevation <= MaxElevation ; CurrentElevation += Time.deltaTime * DistancePersec)
        {
            transform.position += new Vector3(0, Time.deltaTime * DistancePersec, 0);
            yield return null;
        }
    }
}
