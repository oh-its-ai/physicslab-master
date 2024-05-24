using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject objectToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // smoothly move in same direction as objectToFollow but keep own distance and height and rotation
        Vector3 newPos = objectToFollow.transform.position + new Vector3(0, 5, -5);
        // lerp position to new position
        
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);
        transform.LookAt(objectToFollow.transform);
        
    }
}
