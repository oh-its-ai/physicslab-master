using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SpringController : MonoBehaviour
{
    
    public CubeController cubeLeft;
    public CubeController cubeRight;
    
    // CONFIG

    public float stiffness = 169f;
    public float damping = 26f;
    public float mass = 1f;
    public float length = 3f;
    public float springConstant = 1f;
    
    public int swingsUntilYeet = 3;
    public bool letGoCubeRight = false;

    public LineRenderer springLine;
    private float _lastDistance;

    private bool _cubeIsHeadingToSpring;
    private float _lastForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        UpdateSpringVisuals();
        
        /*
        float newDistance = GetDistanceToCubeRight();
        if (newDistance > _lastDistance)
        {
            if (_cubeIsHeadingToSpring)
            {
                _cubeIsHeadingToSpring = false;
            }
        }
        else if(newDistance < _lastDistance)
        {
            if (!_cubeIsHeadingToSpring)
            {
                --swingsUntilYeet;
                _cubeIsHeadingToSpring = true;
            }
        }
        _lastDistance = newDistance;*/
       
        
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
        return;
        if (cubeLeft && cubeRight && springLine)
        {
            
            // Create an array to hold the positions
            Vector3[] positions = new Vector3[2];
        
            // Assign the cube positions to the array
            var position = cubeLeft ? cubeLeft.transform.position : transform.position;
            positions[0] = position;
            positions[1] = position + new Vector3(newLength,0,0) - new Vector3(.5f,0,0);
        
            // Set the positions on the LineRenderer
            springLine.SetPositions(positions);
        }
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
            Vector3 currentPos = springLine.GetPosition(1);
            Vector3 position = Vector3.zero;
            positions[0] = position;
            positions[1] = position + Vector3.Lerp(currentPos, position - new Vector3(length,0,0), Time.deltaTime);
            springLine.SetPositions(positions);
            return;
        }
        
        // Create an array to hold the positions
        
        
        // Assign the cube positions to the array
        if (!cubeLeft)
        {
            Vector3 position = Vector3.zero;
            positions[0] = position;
            positions[1] = position + new Vector3(GetDistanceToCubeRightWithNegatives(),0,0) + new Vector3(.5f,0,0);;
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
