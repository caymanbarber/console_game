using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using System;

public class ComputerMonitor : MonoBehaviour
{
    public Dictionary<String, Program> programs;
    Program currentState; 
    public Material mat;
    public RenderTexture tex;
    public ComputeShader computeShader;
    public Light screenLightL;
    public Light screenLightR;
    public bool videoControl;
    
    void Awake() {
        programs = new Dictionary<string, Program>
        {
            { "CLI", new TypingController(this) },
            { "ScreenSaver", new ScreenSaver(this)},
            { "VideoFeed", new VideoFeed(this)},
            { "Controls", new Controller(this)}
        };
    }

    void Start() {
        EnterState("ScreenSaver");
    }

    void Update(){
        List<Color> colorList = CalculateAverageColorSplit();
        screenLightL.color = colorList[0];
        screenLightR.color = colorList[1];
       //Debug.Log(color);
        currentState?.OnUpdate();
        
    }

    public void EnterState(Program state) {
        if(currentState != null) {
            LeaveState();
        }

        currentState = state;
        currentState.OnStateEnter();
    }

    public void EnterState(String stringState) {
        try {
            EnterState(programs[stringState]);
        } catch (KeyNotFoundException) {
            Debug.Log("Program not found");
        }
    }

    public void LeaveState() {
        currentState.OnStateExit();
    }

    Color CalculateAverageColor() {
        Texture2D tempTexture = new Texture2D(tex.width, tex.height);
        RenderTexture.active = tex;
        tempTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tempTexture.Apply();

        Color avg = new Color(0,0,0);

        for (int w = 0; w < tex.width; w++) {
            for (int h = 0; h < tex.height; h++) {
                avg += tempTexture.GetPixel(w,h);
            }
        }
        return avg/(tex.width*tex.height);
    }

    List<Color> CalculateAverageColorSplit() {
        Texture2D tempTexture = new Texture2D(tex.width, tex.height);
        RenderTexture.active = tex;
        tempTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tempTexture.Apply();

        Color avgL = new Color(0,0,0);
        Color avgR = new Color(0,0,0);

        for (int w = 0; w < tex.width; w++) {
            for (int h = 0; h < tex.height; h++) {
                if(w > tex.width/2)
                    avgR += tempTexture.GetPixel(w,h);
                else    
                    avgL += tempTexture.GetPixel(w,h);
            }
        }

        return new List<Color> {avgL/(tex.width*tex.height/2), avgR/(tex.width*tex.height/2)};
    }


    /*Color CalculateAverageColor() {
         int kernelID = computeShader.FindKernel("CSMain");

        // Create output texture
        outputTexture = new RenderTexture(1, 1, 24); // Adjust the format based on your needs
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();

        computeShader.SetTexture(kernelID, "InputTexture", tex);
        computeShader.SetTexture(kernelID, "OutputTexture", outputTexture);

        // Adjust the thread group size based on your shader
        int numGroupsX = Mathf.CeilToInt(outputTexture.width / 8.0f);
        int numGroupsY = Mathf.CeilToInt(outputTexture.height / 8.0f);

        // Dispatch the compute shader
        computeShader.Dispatch(kernelID, numGroupsX, numGroupsY, 1);

        // Read the result from the output texture
        Color averageColor = ReadAverageColor(outputTexture);

        // Now you have the average color, you can use it as needed
        Debug.Log("Average Color: " + averageColor);
        return averageColor;
    }

    Color ReadAverageColor(RenderTexture rt)
    {
        RenderTexture.active = rt;
        Texture2D texture = new Texture2D(rt.width, rt.height);
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        Color[] pixels = texture.GetPixels();
        return pixels[0];
    }
    */
 }
