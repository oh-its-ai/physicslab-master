using System.Collections;
using System.Collections.Generic;
using Effects;
using ScriptableObjects;
using UnityEngine;

public class Liquid : MonoBehaviour
{
    private Vector3 _startPosition;
    public Vector3 targetPosition;
    private Vector3 _targetPosition;
    public CameraUnderwaterEffect underwaterEffect;
    private MeshRenderer _meshRenderer;
    
    public Color water = new Color(0, 0.42f, 0.87f);
    public Color honey = new Color(0, 0.42f, 0.87f);

    SimulationController Sim => SimulationController.Instance;
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (Sim.GetDataMedium())
        {
            case ScriptableLab.Medium.Water:
                _targetPosition = targetPosition;
                underwaterEffect.depthColor = water;
                _meshRenderer.material.color = water;
                break;
            case ScriptableLab.Medium.Air:
                _targetPosition = _startPosition;
                break;
            case ScriptableLab.Medium.Honey:
                _targetPosition = targetPosition;
                underwaterEffect.depthColor = honey;
                _meshRenderer.material.color = honey;
                break;
            default:
                // Do default stuff
                break;
        }
        
        
        // update plane position
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime);
    }
}
