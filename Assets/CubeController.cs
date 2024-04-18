using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    
    private List<List<float>> _timeSeries;

    public TextMesh labelVelocity;

    private float _lastSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _timeSeries = new List<List<float>>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate() {
        /*var position = _rigidBody.position;
        var forceX = -position.x * springConstant * dirVector.x;
        var forceY = -position.y * springConstant * dirVector.y;
        var forceZ = -position.z * springConstant * dirVector.z;
        if (!(stopAtImpact && _impactRegistered))
        {
            _rigidBody.AddForce(new Vector3(forceX, forceY, forceZ));
        }
        
        _currentTimeStep += Time.deltaTime;
        _timeSeries.Add(new List<float>() {_currentTimeStep, _rigidBody.position.x, _rigidBody.velocity.x, forceX});
        
        
        */
        _timeSeries.Add(new List<float>() {SimulationController.Instance.GetSimTimeInSeconds(), _rigidBody.position.x, GetSpeed(), GetKineticEnergy(), GetImpuls()});
        UpdateText();
    }

    void OnApplicationQuit() {
        WriteTimeSeriesToCSV();
    }

    private void UpdateText()
    {
        if(!labelVelocity) return;
        //labelVelocity.text = textSpeed + "\n" + textEkin;
        labelVelocity.text = "Speed(m/s): " + $"{GetSpeed():0.00}\n" +
                             "E_kin(N): "+  $"{GetKineticEnergy():0.00}\n"+
                             "Impuls: "+  $"{GetImpuls():0.00}\n";
    }

    public float GetKineticEnergy()
    {
        return 0.5f * GetMass() * MathF.Pow(GetSpeed(),2f);
    }

    // Speed is the velocity of the math formulas, so be careful my dear
    public float GetSpeed()
    {
        return Vector3.Magnitude(_rigidBody.velocity);
    }

    public float GetImpuls()
    {
        return GetMass() * GetSpeed();
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

    public Vector3 GetVel()
    {
        return _rigidBody.velocity;
    }

    public float GetMass()
    {
        return _rigidBody.mass;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        _lastSpeed = GetSpeed();
        SimulationController.Instance.EventRegisterImpact(this, other);
        _impactRegistered = true;
            

    }

    public void AddForce(float force)
    {
        _rigidBody.AddForce(new Vector3(force,0,0), ForceMode.Impulse);
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
    
    public float GetTraegheitsmoment()
    {
        return 0.5f * GetMass() * MathF.Pow(GetSpeed(),2f);
    }

    public float GetLastSpeed()
    {
        return _lastSpeed;
    }
}
