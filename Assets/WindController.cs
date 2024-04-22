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
    private float AirDensity => SimulationController.Instance.GetActiveLabConfig().MediumDensity; // kg/m^3
    private Vector3 DragCoefficient => SimulationController.Instance.GetActiveLabConfig().Widerstandsbeiwert; // for e box
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
        /*if (affectedBodies.Count > 0)
        {
            ApplyWindForce();
        }*/
        RotateWindIndicator();
    }

    #endregion
    
    
    public Vector3 GetWindForce()
    {
        return _windForce;
    }
    
    public Vector3 GetWindResistanceForce(CubeController cube, float windSpeed)
    {
        return DragCoefficient * (0.5f * AirDensity * cube.GetArea() * MathF.Pow(cube.GetRidgidBody().velocity.x, 2f));
    }
    
    
    
    public void ApplyWindresistance(Rigidbody body)
    {
        // Implement drag
        var dragCoefficient = _dragCoefficient; // for e box
        var airDensity = AirDensity;
        var area = _area; // Assuming a cube, i know, i know, i could leave it out but i dont want to

        // Calculate drag force like in the BUUKS
        // Fd = 1/2 * Cd * rho * A * v^2 * (de vel inverted and normalized "direction")
        Vector3 dragForce = 0.5f * dragCoefficient * airDensity * area 
                            * MathF.Pow(_windForce.x,2f) 
                            * -_windForce.normalized;
        // print to console: dragCoefficient, airDensity, area, _windForce.magnitude, _windForce.normalized
        Debug.Log("Wind Force: " + _windForce + " - Drag" + dragForce+ " = " + (_windForce - dragForce));
        Debug.Log(body.gameObject.name+ ": "+dragCoefficient + " - " + airDensity + " - " + area + " - " + body.velocity.x + " -> " + dragForce);
        
        body.AddForce(dragForce, ForceMode.Force);
        
    }

    public void EventStartWind()
    {
        _windActive = true;
        windParticles.Play();
    }
    
    public void EventStopWind()
    {
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
