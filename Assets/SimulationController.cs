﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

using System.IO;
using Lab;
using ScriptableObjects;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

/*
    Accelerates the cube to which it is attached, modelling an harmonic oscillator.
    Writes the position, velocity and acceleration of the cube to a CSV file.
    
    Remark: For use in "Physics Engines" module at ZHAW, part of physics lab
    Author: kemf
    Version: 1.0
*/

public class SimulationController : MonoBehaviour
{
    // Config
    public ScriptableLab labConfig;
    public static SimulationController Instance { get; private set; }
    
    // Phases
    public enum Phase {Phase1, Phase2, Phase3, Phase4};

    private LabState _currentState;
    
    [Header("Config Graph")]
    public int graphScalar = 80;
    public int graphOffset = -10;

    private float _springCompression;
    
    [Header("Connections")]
    // GameObjects
    public TextMesh protocolText;
    public Window_Graph windowGraphCube1Vel;
    public Window_Graph windowGraphCube2Vel;
    public SpringController spring1;
    public SpringController springStart;
    public CubeController cube1;
    public CubeController cube2;
    public CubeLController cubeL;
    public TextMesh textMesh1;
    public TextMeshProUGUI uiPhaseText;
    public List<Camera> cameras;
    
    // Data
    private int _lastLoggedSecond = 0;
    private float _msSinceStart = 0f;
    private float _secondsSinceStart = 0;
    public List<int> valueListCube1Vel = new List<int>() {};
    public List<int> valueListCube2Vel = new List<int>() {};

    private Phase _activePhase = Phase.Phase1;
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
        protocolText.text = "Start: " + 0;

        spring1.SetSpringLength(GetActiveLabConfig().springLength);
        
        StartState(GetActiveLabConfig().startingState);
        
        cube1.SetMass(GetActiveLabConfig().cube1Mass);
        cube2.SetMass(GetActiveLabConfig().cube2Mass);
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        _msSinceStart += Time.deltaTime;
        _secondsSinceStart = _msSinceStart % 60;
   
        textMesh1.text = "Time(s): " + $"{_secondsSinceStart:0.00}";

        int secondsSinceStartInt = (int)_secondsSinceStart;
        if (secondsSinceStartInt > _lastLoggedSecond)
        {
            _lastLoggedSecond = (int)_secondsSinceStart;
            int newValue = ((int)(graphScalar * cube1.GetVel().x)) + graphOffset;
            int newValue2 = ((int)(graphScalar * cube2.GetVel().x)) + graphOffset;
            valueListCube1Vel.Add(newValue);
            valueListCube2Vel.Add(newValue2);
            UpdateGraphs();
        }
        
        if(_currentState) _currentState.StateUpdate();
        UpdateSpringForce();
        
    }

    public ScriptableLab GetActiveLabConfig()
    {
        return labConfig;
    }
    private void UpdateSpringForce()
    {
        
        if (GetCubesDistance() <= (GetActiveLabConfig().springLength))
        {
            spring1.SetSpringLength(GetCubesDistance());
            
            _springCompression = GetActiveLabConfig().springLength - GetCubesDistance();
            float force = GetActiveLabConfig().springConstant * _springCompression;
            cube1.AddForce(-force);
            cube2.AddForce(force);
        }
    }

    void OnApplicationQuit() {
        
    }
    
    public void NextCamera()
    {
        foreach (var camera in cameras)
        {
            if (camera.enabled)
            {
                camera.enabled = false;
                int index = cameras.IndexOf(camera);
                if (index == cameras.Count - 1)
                {
                    cameras[0].enabled = true;
                    return;
                }
                cameras[index + 1].enabled = true;
                return;
            }
        }
    }

    public void EventRegisterImpact(CubeController cube, Collision other)
    {
        if(_currentState) _currentState.RegisterEvent(cube, other.gameObject);
    }

    public float GetImpulsPower(CubeController giver)
    {
        return giver.GetMass() * giver.GetSpeed();
    }

    public void WriteProtocol(String text)
    {
        String temp = protocolText.text;
        String newText = "\n" + $"{_secondsSinceStart:0.00}" + ": " + text ;
        protocolText.text = temp + newText;
    }

    public float GetSimTimeInSeconds()
    {
        return _secondsSinceStart;
    }

    public void SetActiveSpring(bool newActive)
    {
        spring1.SetNewActive(newActive);
        spring1.enabled = newActive;
    }

    private void UpdateGraphs()
    {
        windowGraphCube1Vel.ShowGraph(valueListCube1Vel);
        windowGraphCube2Vel.ShowGraph(valueListCube2Vel);
    }
    
    public float GetCubesDistance()
    {
        return Vector3.Distance(cube1.transform.position, cube2.transform.position);
    }

    #region State Handling

   

    private void StartState(LabState newState)
    {
        _currentState = newState;
        _currentState.OnStateEnter();
    }
    public void ChangeState()
    {
        _currentState.OnStateExit();
        _currentState = _currentState.nextState;
        _currentState.OnStateEnter();
        uiPhaseText.text = _currentState.stateName;
    }

    #endregion
    
}
