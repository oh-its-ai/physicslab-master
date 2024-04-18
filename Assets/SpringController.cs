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
        if(!cubeRight) return;
        if(cubeLeft) return;
        float springCompression = length - GetDistanceToCubeRight();
        float force = springConstant * springCompression;


        if (force < 0)
        {
            swingsUntilYeet = 0;
            cubeRight = null;
        }
            
        
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
        if(swingsUntilYeet <= 0)
        {
            Debug.Log("Cube has been yeeted");
            //cubeRight = null;
        }
        else
        {
            
            if(cubeRight)
                cubeRight.AddForce(IsCubeRightToTheRight() ? force : -force);
            Debug.Log("Force: " + (IsCubeRightToTheRight() ? force : -force));
        }
        
    }

    private float GetDistanceToCubeRight()
    {
        return Vector3.Distance(transform.position, cubeRight.transform.position);
    }

    private bool IsCubeRightToTheRight()
    {
        return cubeRight.transform.position.x > transform.position.x;
    }

    public void SetNewActive(bool newActive)
    {
        springLine.enabled = newActive;
    }

    public void SetSpringLength(float newLength)
    {
        if (cubeLeft && cubeRight && springLine)
        {
            length = newLength;
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
        
        if ( !cubeRight || !springLine) return;
        
        // Create an array to hold the positions
        Vector3[] positions = new Vector3[2];
        
        // Assign the cube positions to the array
        if (!cubeLeft)
        {
            Vector3 position = Vector3.zero;
            positions[0] = position;
            positions[1] = position + new Vector3(GetDistanceToCubeRightWithNegatives(),0,0);
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
