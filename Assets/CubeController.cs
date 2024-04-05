using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using DefaultNamespace;

/*
    Accelerates the cube to which it is attached, modelling an harmonic oscillator.
    Writes the position, velocity and acceleration of the cube to a CSV file.
    
    Remark: For use in "Physics Engines" module at ZHAW, part of physics lab
    Author: kemf
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
        labelVelocity.text = _rigidBody.velocity.ToString();
    }

    void OnApplicationQuit() {
        WriteTimeSeriesToCSV();
    }

    private void WriteTimeSeriesToCSV() {
        using (var streamWriter = new StreamWriter("time_series.csv")) {
            streamWriter.WriteLine("t,x(t),v(t),F(t) (added)");
            
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
        CubeController temp;
        if (other.gameObject.TryGetComponent<CubeController>(out temp) && registerCollision)
        {
            SimulationController.Instance.EventRegisterImpact(this);
            _impactRegistered = true;
            // Assume this object also has a Rigidbody component.
            
        }
            
    }

    public void AddForce(float force)
    {
        _rigidBody.AddForce(new Vector3(force,0,0), ForceMode.Impulse);
    }
}
