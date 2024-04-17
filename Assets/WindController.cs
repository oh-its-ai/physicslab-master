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
    private Vector3 _windSpeed;
    public bool windActiveInit = true;
    private bool _windActive = false;
    public GameObject directionIndicator;
    public static WindController Instance { get; private set; }

    public ParticleSystem windParticles;
    public List<Rigidbody> affectedBodies;

    #region Awake, Start
    private void Awake() 
    { 
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
        _windSpeed = SimulationController.Instance.GetActiveLabConfig().GetWindForce();

    }
    

    #endregion

    #region Updates
    
    void FixedUpdate()
    {
        if (affectedBodies.Count > 0)
        {
            ApplyWindForce();
        }
        RotateWindIndicator();
    }

    #endregion
    
    void ApplyWindForce()
    {
        if(!_windActive) return;
        foreach (Rigidbody body in affectedBodies)
        {
            Vector3 newWindSpeed = _windSpeed / body.mass;
            body.AddForce(newWindSpeed, ForceMode.Force);
            
            // Implement drag
            float dragCoefficient = 1.2f; // for e box
            float airDensity = SimulationController.Instance.GetActiveLabConfig().airDensity;
            float area = 1 * 1; // Assuming a cube, i know, i know, i could leave it out but i dont want to

            // Calculate drag force like in the BUUKS Fd = 1/2 * Cd * rho * A * v^2 * (de vel inverted and normalized "direction")
            Vector3 dragForce = 0.5f * dragCoefficient * airDensity * area 
                                * MathF.Pow(body.velocity.magnitude,2f) 
                                * -body.velocity.normalized;

            body.AddForce(dragForce, ForceMode.Force);
        }
    }

    public void EventStartWind()
    {
        SimulationController.Instance.WriteProtocol("Wind started.");
        _windActive = true;
        windParticles.Play();
    }
    
    public void EventStopWind()
    {
        if(_windActive)
            SimulationController.Instance.WriteProtocol("Wind stopped.");
        _windActive = false;
        windParticles.Pause();
    }

    void RotateWindIndicator()
    {
        if (directionIndicator != null && _windSpeed != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_windSpeed);
            directionIndicator.transform.rotation = Quaternion.Lerp(directionIndicator.transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }
}
