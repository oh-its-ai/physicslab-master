using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/LAB", order = 1)]
    public class ScriptableLab : ScriptableObject
    {
        public string labName = "LAB2 V1";

        [Header("Wind")] 
        public Vector3 windDirection = new Vector3(-1, 0, 0);
        public float windSpeed = 20;
        public float airDensity = 1.225f;
        
        [Header("Spring")] 
        public float springLength = 1f;
        public float springConstant = 2f;
        
        [Header("Cube 1")]
        public float cube1Mass = 100;
        
        [Header("Cube 2")]
        public float cube2Mass = 200;

        public Vector3 GetWindForce()
        {
            return windDirection * windSpeed;
        }
    }
}

