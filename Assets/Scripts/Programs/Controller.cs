using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;
using System;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;
using System.Data;

public class Controller : Program
{
    public TMP_Text controlsText;
    public TMP_Text velocityText;
    public TMP_Text RotationText;
    public TMP_Text controlBoolText;
    public float moveForce = 10.0f;
    public float torque = 1.0f;
    public float moveDampRatio = 1f;
    public float rotateDampRatio = 1.5f;
    GameObject gameObject;
    GameObject ship;
    Rigidbody shipRigidbody;
    RayCaster raySource; 
    Sprite[] radarPixels;
    List<List<GameObject>> pixels;
    float pixelSize = 2;
    GameObject radar;
    

    public Dictionary<String,String> controls = new Dictionary<string, string>{
        //control, function
        {"c", "controls"},
        {"escape","exit"},
        {"space", "stabilize"},
        {"shift, ctrl", "vertical"},
        {"w,a,s,d", "fwd, side"},
        {"arrows", "pitch, yaw"},
        {"q,e", "roll"},
        {"enter", "link"}
    };


    public Controller(ComputerMonitor computerMonitor):base(computerMonitor) {
        gameObject = computerMonitor.gameObject;
        Transform controller = gameObject.transform.Find("Controller");
        controlsText = controller.Find("Controls").gameObject.GetComponent<TMP_Text>();
        velocityText = controller.Find("velocities").gameObject.GetComponent<TMP_Text>();
        RotationText = controller.Find("rotations").gameObject.GetComponent<TMP_Text>();
        controlBoolText = controller.Find("ControlBool").gameObject.GetComponent<TMP_Text>();

        ship = GameObject.Find("ship");
        shipRigidbody = ship.GetComponent<Rigidbody>();
        raySource = ship.transform.Find("RaySource").gameObject.GetComponent<RayCaster>();

        radar = controller.Find("Radar").gameObject;

        radarPixels = new Sprite[6];

        Sprite[] all = Resources.LoadAll<Sprite>("RadarPixel");

        foreach( var s in all)
        {
            if (s.name == "1") {
                radarPixels[0] = s;
            } else if (s.name == "2") {
                radarPixels[1] = s;
            } else if (s.name == "3") {
                radarPixels[2] = s;
            } else if (s.name == "4") {
                radarPixels[3] = s;
            } else if (s.name == "5") {
                radarPixels[4] = s;
            } else if (s.name == "6") {
                radarPixels[5] = s;
            }
        }
        
        FillPixels();
    }

    public override void OnUpdate() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            stateController.EnterState("CLI");
        }

        DrawRadar();
        
        if(Input.GetKeyDown(KeyCode.C)) {
            stateController.videoControl = !stateController.videoControl;
            WriteControls(stateController.videoControl);
            if (stateController.videoControl) {
                controlBoolText.text = "[x] controls";
            } else {
                controlBoolText.text = "[ ] controls";
            }
        }
        //TODO: Transform to local z is forward
        WriteVelocities(shipRigidbody.linearVelocity.x, shipRigidbody.linearVelocity.y, shipRigidbody.linearVelocity.z);
        WriteRotations( Mathf.Rad2Deg*shipRigidbody.rotation.x, 
                        Mathf.Rad2Deg*shipRigidbody.rotation.y, 
                        Mathf.Rad2Deg*shipRigidbody.rotation.z);

        if(stateController.videoControl)
            ControlVessel();
    }

    public override void OnStateEnter() {
        gameObject.transform.Find("Controller").gameObject.SetActive(true);
        WriteControls(stateController.videoControl);
        
    }

    public override void OnStateExit() {
        gameObject.transform.Find("Controller").gameObject.SetActive(false);
        
    }

    private void WriteControls(bool controlOn) {
        int longest = 0;

        foreach (String control in this.controls.Keys) {
            longest = longest < control.Length ? control.Length: longest;
        }

        StringBuilder myStringBuilder = new StringBuilder();

        if(!controlOn) {
            String control = "c";
            myStringBuilder.Append("["+control+"]");
            for(int i = 0; i < longest - control.Length + 1; i++) 
                myStringBuilder.Append(" ");
            myStringBuilder.Append(controls[control]);
            controlsText.text = myStringBuilder.ToString();
            return;
        }

        foreach (String control in this.controls.Keys) {
            myStringBuilder.Append("[" + control + "]");
            for(int i = 0; i < longest - control.Length + 1; i++) 
                myStringBuilder.Append(" ");
            myStringBuilder.Append(controls[control]);
            myStringBuilder.Append(System.Environment.NewLine);
        }
        
        controlsText.text = myStringBuilder.ToString();

        
    }

    void WriteVelocities(float x, float y, float z) {
        StringBuilder myStringBuilder = new StringBuilder();

        myStringBuilder.Append("Velocity");
        myStringBuilder.Append("\nx: "+ x.ToString("F"));

        myStringBuilder.Append("\ny: "+y.ToString("F"));

        myStringBuilder.Append("\nz: "+z.ToString("F"));


        velocityText.text = myStringBuilder.ToString();
    }

    void WriteRotations(float x, float y, float z) {
        StringBuilder myStringBuilder = new StringBuilder();

        myStringBuilder.Append("Rotation");
        myStringBuilder.Append("\nx: "+ x.ToString("F"));

        myStringBuilder.Append("\ny: "+y.ToString("F"));

        myStringBuilder.Append("\nz: "+z.ToString("F"));

        RotationText.text = myStringBuilder.ToString();
    }

    void DrawRadar() {
        raySource.DrawRays();
        List<List<float>>  rays = raySource.GetDistanceArray();
        
        for (int row = 0; row < rays.Count; row++) {
            for (int col = 0; col < rays[row].Count; col++) {
                float len = rays[row][col];
                float maxDistance = raySource.GetMaxDistance();
                
                if(len >= maxDistance) {
                    pixels[row][col].SetActive(false);  
                } else if(len < maxDistance && len > 5*maxDistance/6) {
                    pixels[row][col].SetActive(true);
                    pixels[row][col].GetComponent<SpriteRenderer>().sprite = radarPixels[0];
                } else if(len < 5*maxDistance/6 && len > 4*maxDistance/6) {
                    pixels[row][col].SetActive(true);
                    pixels[row][col].GetComponent<SpriteRenderer>().sprite = radarPixels[1];
                }else if(len < 4*maxDistance/6 && len > 3*maxDistance/6) {
                    pixels[row][col].SetActive(true);
                    pixels[row][col].GetComponent<SpriteRenderer>().sprite = radarPixels[3];
                }else if(len < 3*maxDistance/6 && len > 2*maxDistance/6) {
                    pixels[row][col].SetActive(true);
                    pixels[row][col].GetComponent<SpriteRenderer>().sprite = radarPixels[3];
                }else if(len < 2*maxDistance/6 && len > maxDistance/6) {
                    pixels[row][col].SetActive(true);
                    pixels[row][col].GetComponent<SpriteRenderer>().sprite = radarPixels[4];
                }else if(len < maxDistance/6) {
                    pixels[row][col].SetActive(true);
                    pixels[row][col].GetComponent<SpriteRenderer>().sprite = radarPixels[5];
                }
            }
        }
    }

    void FillPixels() {
        int cols = raySource.GetCols();
        int rows = raySource.GetRows();
        pixels = new List<List<GameObject>>();


        

        for (int row = 0; row < rows; row ++) {
            pixels.Add(new List<GameObject>());
            for (int col = 0; col < cols; col ++) {
                float scale = 22f;
                GameObject prefab = Resources.Load<GameObject>("Pixel");
                Vector3 pixelPosition = new Vector3(pixelSize*(cols/2 - col), -pixelSize*(rows/2 - row), -1);
                pixels[row].Add(GameObject.Instantiate(prefab,radar.GetComponent<RectTransform>().position, radar.GetComponent<RectTransform>().rotation));
                pixels[row][col].transform.SetParent(radar.transform);
                pixels[row][col].GetComponent<RectTransform>().localScale = new Vector3(scale,scale,1); 
                pixels[row][col].GetComponent<RectTransform>().position =  radar.GetComponent<RectTransform>().position + ( pixelPosition)/scale;
                pixels[row][col].layer = LayerMask.NameToLayer("UI");
            }
        }
    }

    public void ControlVessel() {

        if(Input.GetKey(KeyCode.Return)) {

            //do link
            //distance < distance

        } else if (Input.GetKey(KeyCode.Space)) {
            float error = 0.1f;
            shipRigidbody.AddForce(-SqrtVec(shipRigidbody.linearVelocity) * moveForce * moveDampRatio);

            shipRigidbody.AddTorque(-SqrtVec(shipRigidbody.angularVelocity) * torque * rotateDampRatio);
            
        } else {
            Vector3 translate = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftControl))
                translate += Vector3.down;

            if (Input.GetKey(KeyCode.LeftShift))
                translate += Vector3.up;

            translate += Input.GetAxisRaw("Horizontal") * Vector3.right;

            translate += Input.GetAxisRaw("Vertical") * Vector3.forward;

            shipRigidbody.AddRelativeForce(translate * moveForce);

            Vector3 rotate = Vector3.zero;

            rotate += Input.GetAxisRaw("Horizontal2") * Vector3.up;

            rotate += Input.GetAxisRaw("Vertical2") * Vector3.left;

            if (Input.GetKey(KeyCode.Q))
                rotate += Vector3.forward;

            if (Input.GetKey(KeyCode.E))
                rotate += -Vector3.forward;


            shipRigidbody.AddRelativeTorque(rotate * torque);
        }
    }

    Vector3 SqrtVec(Vector3 input) {
        return new Vector3(
            Mathf.Sign(input.x) * Mathf.Sqrt(MathF.Abs(input.x)),
            Mathf.Sign(input.y) * Mathf.Sqrt(MathF.Abs(input.y)),
            Mathf.Sign(input.z) * Mathf.Sqrt(MathF.Abs(input.z))
        );
    }
}
