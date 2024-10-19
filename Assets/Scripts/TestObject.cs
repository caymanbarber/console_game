using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 startPosition;
    public Light lightObj;
    [SerializeField]
    float maxIntensity = 10;
    [SerializeField]
    float freq = 5;
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        updateIntensity();
        updatePositiion();
    }

    void updatePositiion() {
        transform.position = startPosition + Vector3.up*Mathf.Sin(Time.time*freq/(2*Mathf.PI)) + Vector3.right*Mathf.Cos(Time.time*freq/(2*Mathf.PI));
    }

    void updateIntensity() {
        lightObj.intensity = maxIntensity * (0.5f + Mathf.Sin(50*Time.time/(Mathf.PI)));
    }
}
