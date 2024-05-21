using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lab;
using ScriptableObjects;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
    private int _lastLoggedSecond;
    private float _msSinceStart;
    private float _secondsSinceStart;
    public List<int> valueListCube1Vel = new List<int>() {};
    public List<int> valueListCube2Vel = new List<int>() {};

    
    // Camera stuff
    private Transform _targetCameraTransform;
    private int _currentCameraIndex;
    private bool _updateGraphs = true;
    
    private Camera _mainCamera;
    private List<List<float>> _timeSeries = new List<List<float>>();
    private ScriptableLab.Medium _medium;
    private float _mediumDensity;
    public Rigidbody jointCubeL;

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
        // load Variables from ScriptableLab
        _medium = labConfig.medium;
        _mediumDensity = labConfig.MediumDensity;
        //protocolText.text = "Start: " + 0;
        _mainCamera = Camera.main;
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
            
            UpdateGraphs();
        }

        LogData();
        
        if(_currentState) _currentState.StateUpdate();
        
    }

    private void Update()
    {
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
        
    }

    void OnApplicationQuit() {
        WriteTimeSeriesToCSV();
        

    }

    #region Camera
    IEnumerator DelayedNextCamera(float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentCameraIndex++;
        _targetCameraTransform = cameras[_currentCameraIndex].transform;
    }
    
    private void UpdateCameraTransform()
    {
        // lerp camera to target
        if (!_mainCamera) return;
        if (!_mainCamera.transform) return;
        if(!_targetCameraTransform) return;
        _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position,
                _targetCameraTransform.position, Time.deltaTime);
        _mainCamera.transform.rotation = Quaternion.Lerp(_mainCamera.transform.rotation,
                _targetCameraTransform.rotation, Time.deltaTime);
        
    }
    
    public void SetUpdateGraphs(bool update)
    {
        _updateGraphs = update;
    }
    public void NextCamera(float delay)
    {
        StartCoroutine(DelayedNextCamera(delay));
    }

    #endregion
    

    public void LogData()
    {
        _timeSeries.Add(new List<float>()
        {
            GetSimTimeInSeconds(),
            cube1.GetSpeed(),
            cube1.GetImpuls(),
            cube1.GetKineticEnergy(),
            cube2.GetSpeed(),
            cube2.GetImpuls(),
            cube2.GetKineticEnergy(),
            spring1.GetSpringForce(),
            cube1.GetKineticEnergy() + cube2.GetKineticEnergy() + spring1.GetSpringForce(),
            cube1.GetImpuls() + cube2.GetImpuls()
        });
    }
    
    public void WriteTimeSeriesToCSV()
    {
        using (var streamWriter = new StreamWriter("sim_time_series.csv"))
        {
            streamWriter.WriteLine("t,v1,p1,F_1,v2,p2,F_2,F_spring,F_total,pTotal");
            foreach (List<float> timeStep in _timeSeries)
            {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
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
        if(!_updateGraphs) return;
        int newValue = ((int)(graphScalar * cube1.GetVel().x)) + graphOffset;
        int newValue2 = ((int)(graphScalar * cube2.GetVel().x)) + graphOffset;
        valueListCube1Vel.Add(newValue);
        valueListCube2Vel.Add(newValue2);
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

    public void LogForces(Vector3 cube1FWind, Vector3 cube1FWindResistance, Vector3 cube1Gravity, Vector3 cube1Normal, Vector3 cube1FTotal, Vector3 cube2FWind, Vector3 cube2FWindResistance, Vector3 cube2Gravity, Vector3 cube2Normal, Vector3 cube2FTotal)
    {
        Debug.Log("Cube1: " + cube1FWind + " " + cube1FWindResistance + " " + cube1Gravity + " " + cube1Normal + " " + cube1FTotal);
        Debug.Log("Cube2: " + cube2FWind + " " + cube2FWindResistance + " " + cube2Gravity + " " + cube2Normal + " " + cube2FTotal);
    }
    
    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    public void TogglePausePlay()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public ScriptableLab.Medium GetDataMedium()
    {
        return _medium;
    }
    
    public float GetDataMediumDensity()
    {
        return _mediumDensity;
    }
    
    public void SwitchMedium(ScriptableLab.Medium newMedium)
    {
        _medium = newMedium;
        _mediumDensity = labConfig.GetMediumDensity(_medium);
    }
    
    public void SwitchMediumWater()
    {
        SwitchMedium(ScriptableLab.Medium.Water);
    }
    
    public void SwitchMediumAir()
    {
        SwitchMedium(ScriptableLab.Medium.Air);
    }
    
    public void SwitchMediumHoney()
    {
        SwitchMedium(ScriptableLab.Medium.Honey);
    }

    public void SetWorldSpeed(float stateWorldSpeed)
    {
        Time.timeScale = stateWorldSpeed;
    }
}
