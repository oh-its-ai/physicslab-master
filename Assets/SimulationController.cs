using System;
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
    Controls the simulation, including the cubes, springs, and camera.
    Author: cavegde1
    Version: 1.0
*/

public class SimulationController : MonoBehaviour
{
    // Config
    public ScriptableLab labConfig;
    public static SimulationController Instance { get; private set; }

    private LabState _currentState;
    
    [Header("Config Graph")]
    public int graphScalar = 80;
    public int graphOffset = -10;

    private float _springCompression;
    
    [Header("Connections")]
    // GameObjects
    public Window_Graph windowGraphCube1Vel;
    public Window_Graph windowGraphCube2Vel;
    public SpringController spring1;
    public SpringController springStart;
    public CubeController cube1;
    public CubeController cube2;
    public CubeLController cubeL;
    public TextMeshProUGUI uiPhaseText;
    public TextMeshProUGUI uiTimeText;
    public TextMeshProUGUI uiInfoText;
    public TextMeshProUGUI uiValuesText;
    public List<Camera> cameras;
    
    // Data
    private int _lastLoggedSecond = 0;
    private float _msSinceStart = 0f;
    private float _secondsSinceStart = 0;
    public List<int> valueListCube1Vel = new List<int>() {};
    public List<int> valueListCube2Vel = new List<int>() {};

    
    // Camera stuff
    private Transform _targetCameraTransform;
    private int _currentCameraIndex;

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
        //protocolText.text = "Start: " + 0;

        spring1.SetSpringLength(GetActiveLabConfig().springLength);
        
        StartState(GetActiveLabConfig().startingState);
        
        cube1.SetMass(GetActiveLabConfig().cube1Mass);
        cube2.SetMass(GetActiveLabConfig().cube2Mass);
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        UpdateTimeWithUI();

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
        UpdateCameraTransform();
    }

    private void UpdateTimeWithUI()
    {
        _msSinceStart += Time.deltaTime;
        _secondsSinceStart = _msSinceStart % 60;
   
        uiTimeText.text = "Time(s): " + $"{_secondsSinceStart:0.00}";
    }


    public ScriptableLab GetActiveLabConfig()
    {
        return labConfig;
    }
    private void UpdateSpringForce()
    {
        return; // disabled for now as it is not needed, I know I could remove it but I dont want to
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

    #region Camera
    IEnumerator DelayedNextCamera(float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentCameraIndex++;
        _targetCameraTransform = cameras[_currentCameraIndex].transform;
        /*
        foreach (var camera in cameras)
        {
            if (camera.enabled)
            {
                camera.enabled = false;
                int index = cameras.IndexOf(camera);
                if (index == cameras.Count - 1)
                {
                    cameras[0].enabled = true;
                    yield break;
                }
                cameras[index + 1].enabled = true;
                yield break;
            }
        }*/
    }
    
    private void UpdateCameraTransform()
    {
        // lerp camera to target
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _targetCameraTransform.position, Time.deltaTime);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, _targetCameraTransform.rotation, Time.deltaTime);
    }
    
    public void NextCamera(float delay)
    {
        StartCoroutine(DelayedNextCamera(delay));
    }

    #endregion
    

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
        String temp = uiInfoText.text;
        String newText = "\n" + $"{_secondsSinceStart:0.00}" + ": " + text ;
        uiInfoText.text = temp + newText;
    }
    
    public void WriteValues(String text)
    {
        String temp = uiValuesText.text;
        String newText = "\n" + $"{_secondsSinceStart:0.00}" + ": " + text ;
        uiValuesText.text = temp + newText;
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
