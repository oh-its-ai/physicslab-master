using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public float windForceFactor = 1;
    private Cloth _cloth;
    private WindController WindController => WindController.Instance;
    private void Awake()
    {
        _cloth = GetComponent<Cloth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 windForce = windForceFactor * WindController.GetGeneralWindForce();
        _cloth.externalAcceleration = windForce;
    }
}
