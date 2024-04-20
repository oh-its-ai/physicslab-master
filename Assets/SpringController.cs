using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SpringController : MonoBehaviour
{
    
    public CubeController cubeLeft;
    public CubeController cubeRight;
    
    public GameObject middle;
    
    // CONFIG
    
    public float length = 3f;
    public float springConstant = 1f;
    
    public bool letGoCubeRight = false;

    public LineRenderer springLine;
    public LineRenderer middleLine;
    private float _lastDistance;

    private bool _cubeIsHeadingToSpring;
    private float _lastForce;
    private Vector3 _newSpringMiddle;
    private Vector3 _initMiddle;

    private void Start()
    {
        if (middle)
        {
            Vector3 springMiddle = (cubeLeft.transform.position + cubeRight.transform.position) / 2;
            float cubeDistanceCubeLtoCuber = Vector3.Distance(cubeLeft.transform.position, cubeRight.transform.position);
            if (cubeDistanceCubeLtoCuber > length)
            {
                springMiddle = cubeLeft.transform.position + new Vector3(length/2, 0, 0);
            }
            middle.transform.position = springMiddle;
            _initMiddle = springMiddle;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateSpringMiddle();
        UpdateSpringVisuals();
    }

    private void UpdateSpringMiddle()
    {
        if (middle)
        {
            Vector3 springMiddle = (cubeLeft.transform.position + cubeRight.transform.position) / 2;
            float cubeDistanceCubeLtoCuber = Vector3.Distance(cubeLeft.transform.position, cubeRight.transform.position);
            if (cubeDistanceCubeLtoCuber > length)
            {
                springMiddle = cubeLeft.transform.position + new Vector3(length/2, 0, 0);
            }
            middle.transform.position = springMiddle;
            if (middleLine)
            {
                Vector3[] positions = new Vector3[2];
                Vector3 position = _initMiddle;
                positions[0] = position;
                positions[1] = springMiddle;
                middleLine.SetPositions(positions);
            }
        }
    }


    public float GetDistanceToCubeRight()
    {
        return Vector3.Distance(transform.position, cubeRight.transform.position);
    }

    public bool IsCubeRightToTheRight()
    {
        return cubeRight.transform.position.x > transform.position.x;
    }

    public void SetNewActive(bool newActive)
    {
        springLine.enabled = newActive;
    }

    public void SetSpringLength(float newLength)
    {
        length = newLength;
    }

    public void ReleaseCubeRight()
    {
        if (letGoCubeRight)
        {
            cubeRight = null;
        }
    }
    private void UpdateSpringVisuals()
    {
        Vector3[] positions = new Vector3[2];
        if (!cubeRight || !springLine)
        {
            return;
            Vector3 currentPos = springLine.GetPosition(1);
            Vector3 position = Vector3.zero;
            positions[0] = position;
            positions[1] = position + Vector3.Lerp(currentPos, position - new Vector3(length,0,0), Time.deltaTime);
            springLine.SetPositions(positions);
            return;
        }
        
        // Assign the cube positions to the array
        if (!cubeLeft)
        {
            Vector3 position = Vector3.zero;
            positions[0] = position;
            positions[1] = position + new Vector3(GetDistanceToCubeRightWithNegatives(),0,0) + new Vector3(.5f,0,0);
        }
        else
        {
            var position = cubeLeft.transform.position;
            positions[0] = position + new Vector3(.5f,0,0);
            positions[1] = position + new Vector3(length,0,0) - new Vector3(.5f,0,0);
        }
        
        
        // Set the positions on the LineRenderer
        springLine.SetPositions(positions);
    }

    private float GetDistanceToCubeRightWithNegatives()
    {
        return IsCubeRightToTheRight() ? GetDistanceToCubeRight() : -GetDistanceToCubeRight();
    }
}
