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
    private float MediumDensity => SimulationController.Instance.GetDataMediumDensity(); // kg/m^3
    private float DragCoefficient => SimulationController.Instance.GetActiveLabConfig().Widerstandsbeiwert; // for e box
    
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
    
    public Vector3 GetWindResistanceForce(CubeController cube,Vector3 windDirection, float windSpeed)
    {
        var relativeVelocity = (windDirection*windSpeed) - cube.GetRidgidBody().velocity;
        var speed = relativeVelocity.magnitude;
        float drag = DragCoefficient * (0.5f * MediumDensity * cube.GetArea() * MathF.Pow(speed, 2f));
        return cube.GetRidgidBody().velocity.normalized * drag;
    }
    
    public Vector3 GetWindForce(CubeController cube, Vector3 windDirection, float windSpeed)
    {
        var relativeVelocity = (windDirection*windSpeed) - cube.GetRidgidBody().velocity;
        var speed = relativeVelocity.magnitude;
        var windForce = 0.5f * DragCoefficient * MediumDensity * cube.GetArea() * MathF.Pow(speed,2) * relativeVelocity.normalized;
        return windForce;
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

    

    public void SetWindForce(Vector3 windForce)
    {
        _windForce = windForce;
    }
    public Vector3 GetGeneralWindForce()
    {
        return _windForce;
    }
}
