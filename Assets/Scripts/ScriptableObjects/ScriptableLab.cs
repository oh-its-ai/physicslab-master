using Lab;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/LAB", order = 1)]
    public class ScriptableLab : ScriptableObject
    {
        public string labName = "LAB2 V1";

        public LabState startingState;
        
        [Header("Wind")] 
        [Tooltip("Wind direction in m/s")]
        public Vector3 windDirection = new Vector3(-1, 0, 0); // Vector m/s
        [Tooltip("Wind speed in m/s")]
        public float windSpeed = 20; // m/s
        [Tooltip("Medium density in kg/m^3 (Air: 1.225, Water: 1000, Oil: 900, Honey: 1400)")]
        public Medium medium = Medium.Air;
        public ObjectType objektForm = ObjectType.Cube;
        public float MediumDensity => GetMediumDensity();
        public float Widerstandsbeiwert => GetWidersandsbeiwert();

        
        
        [Header("Spring Start")]
        [Tooltip("Spring length in meters.")]
        public float springStartLength = 1f; // m
        [Tooltip("Spring constant in N/m")]
        public float springStartConstant = 2f; // N/m
        
        [Header("Spring 1")] 
        [Tooltip("Spring length in meters.")]
        public float springLength = 1f; // m
        [Tooltip("Spring constant in N/m")]
        public float springConstant = 2f; // N/m
        
        [Header("Cube 1")]
        [Tooltip("Cube 1 mass in kg.")]
        public float cube1Mass = 100; // kg
        
        [Header("Cube 2")]
        [Tooltip("Cube 2 mass in kg.")]
        public float cube2Mass = 200; // kg

        public enum Medium
        {
            Air,
            Water,
            Oil,
            Honey
        }
        
        public enum ObjectType
        {
            Cube,
            Sphere,
            Cylinder
        }
        
        private float GetMediumDensity()
        {
            switch (medium)
            {
                case Medium.Air:
                    return 1.225f;
                case Medium.Water:
                    return 1000f;
                case Medium.Oil:
                    return 900f;
                case Medium.Honey:
                    return 1400f;
                default:
                    return 1.225f;
            }
        }
        
        private float GetWidersandsbeiwert()
        {
            switch (objektForm)
            {
                case ObjectType.Cube:
                    return 1.2f;
                case ObjectType.Sphere:
                    return 0.47f;
                case ObjectType.Cylinder:
                    return 0.82f;
                default:
                    return 1.2f;
            }
        }
        
        public Vector3 GetWindForce()
        {
            return windDirection * windSpeed;
        }
    }
}

