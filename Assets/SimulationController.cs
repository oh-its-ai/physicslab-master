﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

using System.IO;

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
    public Window_Graph WindowGraph;
    
    // CUBE 1 Stuff
    public CubeController Cube1;
    public TextMesh TextMesh1;
    
    private Vector3 Cube1_Vel;
    
    // Data
    private int _lastLoggedSecond = 0;
    private float _msSinceStart = 0f;
    private float _secondsSinceStart = 0;
    public List<int> valueList = new List<int>() {};
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
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        _msSinceStart += Time.deltaTime;
        _secondsSinceStart = _msSinceStart % 60;
        Cube1_Vel = Cube1.GetVel();
        TextMesh1.text = Cube1_Vel.ToString();

        int secondsSinceStartInt = (int)_secondsSinceStart;
        if (secondsSinceStartInt > _lastLoggedSecond)
        {
            _lastLoggedSecond = (int)_secondsSinceStart;
            int newValue = ((int)(GraphScalar * Cube1_Vel.x)) + GraphOffset;
            valueList.Add(newValue);
            UpdateGraphs();
        }
        
    }

    void OnApplicationQuit() {
        
    }

    public void EventRegisterImpact(CubeController cube)
    {
        WindController.Instance.EventStopWind();
        WriteProtocol(cube.name);
    }

    public void WriteProtocol(String text)
    {
        String temp = ProtocolText.text;
        String newText = "\n" + $"{_secondsSinceStart:0.00}" + ": " + text ;
        ProtocolText.text = temp + newText;
    }

    private void UpdateGraphs()
    {
        WindowGraph.ShowGraph(valueList);
    }
    
}
