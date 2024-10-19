using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    // Start is called before the first frame update
    int rows = 40;
    int cols = 60;
    float yAngle = 20f;
    float xAngle = 30f;
    float maxDistance = 50;
    List<List<float>> distanceArray;

    void Start()
    {
        SetResolution(cols,rows);
    }

    // Update is called once per frame
    void Update()
    {
        DrawRays();
    }
    public void DrawRays() {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;

        for(int row = 0; row < rows; row++) {
            for(int col = 0; col < cols; col++) {
                RaycastHit hit;
                float normalizedY = (rows/2 - row)/(float)rows;
                float normalizedX = (cols/2 - col)/(float)cols;

                
                Quaternion rotationx = Quaternion.AngleAxis(xAngle * normalizedX , transform.up);
                Quaternion rotationy = Quaternion.AngleAxis(yAngle * normalizedY , transform.right);
                
                LayerMask raycastable =  ~LayerMask.GetMask("NotRayCastable");
                
                if (Physics.Raycast(transform.position, rotationx * rotationy * transform.forward , out hit, maxDistance, raycastable)) {
                    distanceArray[row][col] = hit.distance;
                } else {
                    distanceArray[row][col] = maxDistance;
                }
            }
        }
    }

    public void SetResolution(int x, int y) {
        rows = y;
        cols = x;

        distanceArray = new List<List<float>>();
        for(int row = 0; row < rows; row ++) {
            distanceArray.Add(new List<float>());
            for(int col = 0; col < cols; col ++) {
                distanceArray[row].Add(maxDistance);
            }
        }
    }


    public void SetAngle(int x, int y) {
        xAngle = x;
        yAngle = y;
    }

    public int GetRows() {
        return rows;
    }

    public int GetCols() {
        return cols;
    }

    public float GetMaxDistance() {
        return maxDistance;
    }

    public List<List<float>>  GetDistanceArray() {
        return distanceArray;
    }
}
