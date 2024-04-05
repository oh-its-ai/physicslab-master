using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

using System.IO;

/*
    Accelerates GOs like wind
    
    Author: cavegde1
    Version: 1.0
*/
public class WindController : MonoBehaviour
{
    // CONFIG
    public Vector3 windSpeed;
    public bool windActiveInit = true;
    private bool _windActive = false;
    public GameObject directionIndicator;
    public static WindController Instance { get; private set; }

    public List<Rigidbody> affectedBodies;
   
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
        _windActive = windActiveInit;
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        if (affectedBodies.Count > 0)
        {
            ApplyWindForce();
        }

        RotateWindIndicator();
    }
    
    
    void ApplyWindForce()
    {
        if(!_windActive) return;
        // Apply wind force to all affected rigidbodies
        foreach (Rigidbody body in affectedBodies)
        {
            body.AddForce(windSpeed, ForceMode.Force);
        }
    }

    public void EventStartWind()
    {
        SimulationController.Instance.WriteProtocol("Wind started.");
        _windActive = true;
    }
    
    public void EventStopWind()
    {
        if(_windActive)
            SimulationController.Instance.WriteProtocol("Wind stopped.");
        _windActive = false;
    }

    void RotateWindIndicator()
    {
        if (directionIndicator != null && windSpeed != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(windSpeed);
            directionIndicator.transform.rotation = Quaternion.Lerp(directionIndicator.transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }
}
