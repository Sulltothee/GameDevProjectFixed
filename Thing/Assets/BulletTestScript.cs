using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
        gameObject.GetComponent<Rigidbody>().AddForce(transform.up * 100);
    }
}
