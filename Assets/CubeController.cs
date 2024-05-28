using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;

/*
    Accelerates the cube to which it is attached, modelling an harmonic oscillator.
    Writes the position, velocity and acceleration of the cube to a CSV file.
    
    Remark: For use in "Physics Engines" module at ZHAW, part of physics lab
    Author: kemf, cavegde1
    Version: 1.0
*/
public class CubeController : MonoBehaviour 
{
    // Config
    public bool registerCollision = false;
    public bool stopAtImpact = false;
    private bool _impactRegistered = false;
    private Rigidbody _rigidBody;

    public int springConstant; // N/m
    public Vector3 dirVector;
    private float _currentTimeStep; // s
    private Vector3 _startingPosition;
    
    private List<List<float>> _timeSeries;

    public TextMeshProUGUI infoText;

    private float _lastSpeed;
    private Vector3 _previousPosition;
    private float _totalDistance;
    
    private List<Vector3> _velHistory = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _timeSeries = new List<List<float>>();
        _previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate() {
        var distanceThisFrame = Vector3.Distance(transform.position, _previousPosition);
        _totalDistance += distanceThisFrame;
        _previousPosition = transform.position;
        _timeSeries.Add(new List<float>() {SimulationController.Instance.GetSimTimeInSeconds(), _rigidBody.position.x, GetSpeed(), GetKineticEnergy(), GetImpuls()});
        _velHistory.Add(_rigidBody.velocity);
        UpdateText();
    }

    void OnApplicationQuit() {
        WriteTimeSeriesToCSV();
    }

    private void UpdateText()
    {
        if(!infoText) return;
        infoText.text = name + "\n"+ 
                        "Speed(m/s)(v): " + $"{GetSpeed():0.00}\n" +
                        "E_kin(N): "+  $"{GetKineticEnergy():0.00}\n"+
                        "Impuls(p): "+  $"{GetImpuls():0.00}\n" + 
                        "F_WindRes(N)=" + $"{GetWindResistanceForce():0.00}|";;
    }

    #region Get Attributes

    public float GetKineticEnergy()
    {
        return 0.5f * GetMass() * MathF.Pow(GetSpeed(),2f);
    }

    // Speed is the velocity of the math formulas, so be careful my dear
    public float GetSpeed()
    {
        return _rigidBody.velocity.x;
    }

    public float GetImpuls()
    {
        return GetMass() * GetSpeed();
    }

    public float GetDistanceTravelled()
    {
        return _totalDistance;
    }

    public Vector3 GetVel()
    {
        return _rigidBody.velocity;
    }

    public float GetMass()
    {
        return _rigidBody.mass;
    }
    
    public float GetTraegheitsmoment()
    {
        return 0.5f * GetMass() * MathF.Pow(GetSpeed(),2f);
    }

    public float GetLastSpeed()
    {
        return _lastSpeed;
    }
    
    public void SetKinematic(bool value)
    {
        // set kinematic
        GetRidgidBody().isKinematic = value;
    }
    
    public float GetWeight()
    {
        return GetMass() * Physics.gravity.y;
    }
    
    public float GetNormalForce(float normal)
    {
        return GetMass() * normal;
    }
    
    public Vector3 GetNormalForceVector3(float winklePlane)
    {
        return GetMass() * Physics.gravity * Mathf.Cos(winklePlane);
    }
    
    #endregion

    #region Utilities

    public String GetCubeDataText()
    {
        return name + ": p=" + $"{GetImpuls():0.00} kg/s |" 
               + " v=" + $"{GetSpeed():0.00} m/S |" 
               + " s=" + $"{GetDistanceTravelled():0.00} m |"
               + " E_kin=" + $"{GetKineticEnergy():0.00} N |"
               + " F_Luft=" + $"{GetWindResistanceForce():0.00} N |";
    }

    private float GetWindResistanceForce()
    {
        return WindController.Instance.GetWindResistanceForce(this, Vector3.zero, 0f).x;
    }

    private void WriteTimeSeriesToCSV() {
        using (var streamWriter = new StreamWriter(name + "time_series.csv")) {
            streamWriter.WriteLine("t,x(t),v(t),F(t),p(added)");
            
            foreach (List<float> timeStep in _timeSeries) {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }
    
    
    public void AddForce(float force)
    {
        // bro listen, you dont need to take mass into account here
        _rigidBody.AddForce(new Vector3(force,0,0));
    }

    public void SetMass(float cubeMass)
    {
        _rigidBody.mass = cubeMass;
    }
    
    public void AttachTo(GameObject target)
    {
        transform.parent = target.transform;
    }

    public void DisableRigidbody()
    {
        _rigidBody.isKinematic = true;
    }
    
    #endregion

    
    public Vector3 GetMaxSpeed()
    {
        // return vector furthest away from 0
        return _velHistory.Aggregate((i1, i2) => i1.magnitude > i2.magnitude ? i1 : i2);
        
    }
    

    
    
    private void OnCollisionEnter(Collision other)
    {
        _lastSpeed = GetSpeed();
        SimulationController.Instance.EventRegisterImpact(this, other);
        _impactRegistered = true;
    }


    public Rigidbody GetRidgidBody()
    {
        return _rigidBody;
    }

    public float GetArea()
    {
        return 1 * 1; // Assuming a cube, i know, i know, i could leave it out but i dont want to
    }

    public void AddToJoint(Rigidbody jointCubeL)
    {
        var joint = gameObject.AddComponent<FixedJoint>();
        joint.massScale = 200;
        joint.connectedBody = jointCubeL;
    }

    public void DisableCollider()
    {
        GetComponent<Collider>().enabled = false;
    }
}
