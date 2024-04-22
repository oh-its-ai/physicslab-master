using System;
using System.Collections.Generic;
using UnityEngine;

/*
    Accelerates Cubes like wind
    
    Author: cavegde1
    Version: 1.0
*/
public class WindController : MonoBehaviour
{
    // CONFIG
    private Vector3 _windForce;
    public bool windActiveInit = true;
    private bool _windActive = false;
    public GameObject directionIndicator;
    public static WindController Instance { get; private set; }

    public ParticleSystem windParticles;
    public List<Rigidbody> affectedBodies;
    
    // Config
    private float _airDensity = SimulationController.Instance.GetActiveLabConfig().MediumDensity; // kg/m^3
    private float _dragCoefficient = 1.2f; // for e box
    private float _area = 1 * 1; // Assuming a cube, i know, i know, i could leave it out but i dont want to
    
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
        _windForce = SimulationController.Instance.GetActiveLabConfig().GetWindForce();

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
            Vector3 newWindForce = _windForce;
            body.AddForce(newWindForce, ForceMode.Force);
            ApplyWindresistance(body);
        }
    }
    
    public void ApplyWindForceToBody(Rigidbody body)
    {
        if(!_windActive) return;
        Vector3 newWindForce = _windForce;
        body.AddForce(newWindForce, ForceMode.Force);
        ApplyWindresistance(body);
    }
    
    public void ApplyWindresistance(Rigidbody body)
    {
        // Implement drag
        var dragCoefficient = _dragCoefficient; // for e box
        var airDensity = _airDensity;
        var area = _area; // Assuming a cube, i know, i know, i could leave it out but i dont want to

        // Calculate drag force like in the BUUKS
        // Fd = 1/2 * Cd * rho * A * v^2 * (de vel inverted and normalized "direction")
        Vector3 dragForce = 0.5f * dragCoefficient * airDensity * area 
                            * MathF.Pow(_windForce.magnitude,2f) 
                            * -_windForce.normalized;

        
        body.AddForce(dragForce, ForceMode.Force);
        
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
        windParticles.Stop();
    }

    void RotateWindIndicator()
    {
        if (directionIndicator && _windForce != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_windForce);
            directionIndicator.transform.rotation = Quaternion.Lerp(directionIndicator.transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }

    public void SetWind(Vector3 windDirection, float windSpeed)
    {
        _windForce = windDirection.normalized * windSpeed;
    }
}
