using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

using System.IO;
using UnityEngine.Serialization;

/*
    Accelerates the cube to which it is attached, modelling an harmonic oscillator.
    Writes the position, velocity and acceleration of the cube to a CSV file.
    
    Remark: For use in "Physics Engines" module at ZHAW, part of physics lab
    Author: kemf
    Version: 1.0
*/
public class SimulationController : MonoBehaviour
{
    // CONFIG
    public Vector3 windSpeed;
    public static SimulationController Instance { get; private set; }

    public int GraphScalar = 80;
    public int GraphOffset = -10;

    // GameObjects
    public TextMesh ProtocolText;
    public Window_Graph windowGraphCube1Vel;
    public Window_Graph windowGraphCube2Vel;
    // CUBE 1 Stuff
    public CubeController Cube1;
    public CubeController Cube2;
    public TextMesh TextMesh1;
    
    private Vector3 Cube1_Vel;
    
    // Data
    private int _lastLoggedSecond = 0;
    private float _msSinceStart = 0f;
    private float _secondsSinceStart = 0;
    public List<int> valueListCube1Vel = new List<int>() {};
    public List<int> valueListCube2Vel = new List<int>() {};
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private void Start()
    {
        ProtocolText.text = "Start: " + 0;
        WindController.Instance.EventStartWind();
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        _msSinceStart += Time.deltaTime;
        _secondsSinceStart = _msSinceStart % 60;
        Cube1_Vel = Cube1.GetVel();
        TextMesh1.text = "Time(s): " + $"{_secondsSinceStart:0.00}";

        int secondsSinceStartInt = (int)_secondsSinceStart;
        if (secondsSinceStartInt > _lastLoggedSecond)
        {
            _lastLoggedSecond = (int)_secondsSinceStart;
            int newValue = ((int)(GraphScalar * Cube1.GetVel().x)) + GraphOffset;
            int newValue2 = ((int)(GraphScalar * Cube2.GetVel().x)) + GraphOffset;
            valueListCube1Vel.Add(newValue);
            valueListCube2Vel.Add(newValue2);
            UpdateGraphs();
        }
        
    }

    void OnApplicationQuit() {
        
    }

    public void EventRegisterImpact(CubeController cube)
    {
        WindController.Instance.EventStopWind();
        //WriteProtocol(cube.name);
    }

    public void WriteProtocol(String text)
    {
        String temp = ProtocolText.text;
        String newText = "\n" + $"{_secondsSinceStart:0.00}" + ": " + text ;
        ProtocolText.text = temp + newText;
    }

    private void UpdateGraphs()
    {
        windowGraphCube1Vel.ShowGraph(valueListCube1Vel);
        windowGraphCube2Vel.ShowGraph(valueListCube2Vel);
    }
    
}
