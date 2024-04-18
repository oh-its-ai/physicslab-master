using UnityEngine;
using UnityEngine.Serialization;

public class CubeLController : CubeController
{
        private float _angleSpeed;
        private float _initImpuls;
        public GameObject fixedJoint;
        public GameObject cube1;
        public GameObject cube2;
        public GameObject cube3;
        
        public float lengthSingleCube;
        

        public void CalculateAngleSpeed()
        {
            
        }
        
        public new float GetTraegheitsmoment()
        {
            // calc traegheitsmoment eines L körpers
            float I_wuerfel = (1f / 6f) * massSingleCube * (Mathf.Pow(lengthSingleCube, 2));


            return I_wuerfel;
        }
        
        public float GetDistanceCubeToAxis()
        {
            // calc distance of cube to axis
            return Vector3.Distance(cube1.transform.position, fixedJoint.transform.position);
        }

        public Vector3 GetSchwerpunkt()
        {
            // calc center of mass
            return (cube1.transform.position + cube2.transform.position + cube3.transform.position) / 3;
        }

        public int massSingleCube { get; set; }

        public float GetTraegheitsmomentCube()
        {
            throw new System.NotImplementedException();
        }
}