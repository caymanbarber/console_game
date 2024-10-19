using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscare : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ship;
    public float speed = 10;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Magnitude(ship.transform.position-transform.position)<=50 ) {
            
            transform.Translate(new Vector3(0,0,-speed)*Time.deltaTime);
        }
    }
}
