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

    public LineRenderer springLine;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpringVisuals();
    }

    public void Reset()
    {
        
    }

    public void SetSpringLength(float newLength)
    {
        if (cubeLeft && cubeRight && springLine)
        {
            length = newLength;
            // Create an array to hold the positions
            Vector3[] positions = new Vector3[2];
        
            // Assign the cube positions to the array
            var position = cubeLeft.transform.position;
            positions[0] = position;
            positions[1] = position + new Vector3(newLength,0,0);
        
            // Set the positions on the LineRenderer
            springLine.SetPositions(positions);
        }
    }

    private void UpdateSpringVisuals()
    {
        if (cubeLeft && cubeRight && springLine)
        {
            // Create an array to hold the positions
            Vector3[] positions = new Vector3[2];
        
            // Assign the cube positions to the array
            positions[0] = cubeLeft.transform.position;
            positions[1] = cubeLeft.transform.position + new Vector3(length,0,0);
        
            // Set the positions on the LineRenderer
            springLine.SetPositions(positions);
        }
    }
}
