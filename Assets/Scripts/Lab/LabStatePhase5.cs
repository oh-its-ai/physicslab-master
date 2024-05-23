using System;
using System.Collections.Generic;
using UnityEngine;
/*
 * GOAL: Berechnung des Drehimpulses des L-Körpers
 * Berechnenung des Drehimpulses des Würfels 2
 * Berechnung des Drehimpulses des gesamten Systems
 * 
 */
namespace Lab
{
    [CreateAssetMenu(fileName = "Phase5_", menuName = "Phasen/Phase5", order = 1)]
    public class LabStatePhase5 : LabStatePhase4
    {
        private CubeLController _cubeL;
        private CubeController _cube2;
        
        private float _traegheitsmomentCube2;
        private float _traegheitsmomentCubeL;
        private float _angleSpeed = 0;

        private Vector3 JointOrigin => _cubeL.fixedJoint.transform.position;
        private Vector3 _hitJointPos;
        
        private Rigidbody _rbCubeLCube1;
        private Rigidbody _rbCubeLCube2;
        private Rigidbody _rbCubeLCube3;
        private Vector3 _bahnDrehImpuls;
        private Vector3 _cube2HitImpuls;
        
        private List<LBodyPart> _lBodyParts = new List<LBodyPart>();
        private Vector3 _cube2HitDir;

        public override void OnStateEnter()
        {
            
            Sim.SetWorldSpeed(stateWorldSpeed);
            Sim.SetUpdateGraphs(false);
            _cubeL = Sim.cubeL;
            _cube2 = Sim.cube2;
            
            _rbCubeLCube1 = _cubeL.cube1.GetComponent<Rigidbody>();
            _rbCubeLCube2 = _cubeL.cube2.GetComponent<Rigidbody>();
            _rbCubeLCube3 = _cubeL.cube3.GetComponent<Rigidbody>();
            
            _lBodyParts.Add(new LBodyPart(_rbCubeLCube1, CubePos.Mitte));
            _lBodyParts.Add(new LBodyPart(_rbCubeLCube2, CubePos.Aussen));
            _lBodyParts.Add(new LBodyPart(_rbCubeLCube3, CubePos.Mitte));
            _lBodyParts.Add(new LBodyPart(_cube2.GetRidgidBody(), CubePos.Aussen));

            //_cube2HitImpuls = _cube2.GetRidgidBody().velocity * _cube2.GetMass();
            _cube2HitImpuls = _cube2.GetMaxSpeed() * _cube2.GetMass();
            _cube2HitDir = _cube2.GetMaxSpeed().normalized;
            
            // GOAL 1 BahnDrehImpuls des Würfels 2
            _hitJointPos = JointOrigin;
            _bahnDrehImpuls = CalcAngularMomentum(_cube2.GetRidgidBody(), _hitJointPos);
            
            // register hit
            _cube2.AddToJoint(JointCubeL);
            _cube2.DisableCollider();
        }

        public override void StateUpdate()
        {
            

            Vector3 amLCube1 = CalcAngularMomentum(_rbCubeLCube1, JointCubeL.transform.position);
            Vector3 amLCube2 = CalcAngularMomentum(_rbCubeLCube2, JointCubeL.transform.position);
            Vector3 amLCube3 = CalcAngularMomentum(_rbCubeLCube3, JointCubeL.transform.position);
            Vector3 amCube2 = CalcAngularMomentum(_cube2.GetRidgidBody(), JointCubeL.transform.position);
            
            Vector3 sumAm = amLCube1 + amLCube2 + amLCube3 + amCube2;
            Debug.Log("Summe Momentum: " + sumAm);
            
            float tgmLCube1 = CalcTraegheitsMoment(_rbCubeLCube1, CubePos.Mitte);
            float tgmLCube2 = CalcTraegheitsMoment(_rbCubeLCube2, CubePos.Aussen);
            float tgmLCube3 = CalcTraegheitsMoment(_rbCubeLCube3, CubePos.Mitte);
            float tgmCube2 = CalcTraegheitsMoment(_cube2.GetRidgidBody(), CubePos.Aussen);
            
            float sumTgm = CalcSumTraegheitsMoment(_lBodyParts);
            Debug.Log("Summe Traegheitsmoment: " + sumTgm);

            Vector3 drehImpuls = sumTgm * _cube2.GetRidgidBody().angularVelocity;
            
            _bahnDrehImpuls = CalcAngularMomentum(_cube2.GetRidgidBody(), _hitJointPos);
            Debug.Log("Bahndrehimpuls: " + _bahnDrehImpuls + " : Mag: " + _bahnDrehImpuls.magnitude);
            Debug.Log("EigenDrehimpuls V3: " + drehImpuls + " : Mag: " + drehImpuls.magnitude);
            
            Debug.Log("L Mass: " + CalcLBodyMass(_lBodyParts) + " Vel: " + CalcTranslationVel(_lBodyParts));
            Debug.Log("Wuerlfel 2 Hit Impuls: " + _cube2HitImpuls + " : Mag: " + _cube2HitImpuls.magnitude);

            Vector3 translationImpuls = JointCubeL.velocity * 800;
            Debug.Log("L Koerper Translatorischer Impuls: " + translationImpuls + " : Mag: " + translationImpuls.magnitude);
            
            
            Sim.drehImpuls = drehImpuls.magnitude;
        }
        
        private Vector3 CalcTranslationVel(List<LBodyPart> lBodyParts)
        {
            Vector3 sum = Vector3.zero;
            foreach (LBodyPart lBodyPart in lBodyParts)
            {
                sum += lBodyPart.rb.velocity;
            }
            
            // sum /= lBodyParts.Count;

            return sum / lBodyParts.Count;
        }
        
        private float CalcLBodyMass(List<LBodyPart> lBodyParts)
        {
            float sum = 0;
            foreach (LBodyPart lBodyPart in lBodyParts)
            {
                sum += lBodyPart.rb.mass;
            }

            return sum;
        }
        
        private float CalcTranslationImpuls(List<LBodyPart> lBodyParts)
        {
            float sum = 0;
            foreach (LBodyPart lBodyPart in lBodyParts)
            {
                sum += lBodyPart.rb.mass * lBodyPart.rb.velocity.magnitude;
            }

            return sum;
        }

        private Vector3 CalcTranslationImpulsV3(List<LBodyPart> lBodyParts)
        {
            Vector3 sum = Vector3.zero;
            foreach (LBodyPart lBodyPart in lBodyParts)
            {
                sum += lBodyPart.rb.mass * lBodyPart.rb.velocity;
            }

            return sum;
        }
        
        public float CalcAngleSpeed()
        {
            float I_cubeL = _cubeL.GetTraegheitsmoment() + _cubeL.GetMass()* _cubeL.GetDistanceCubeToAxis();
            Debug.Log("I_cubeL: " + I_cubeL);
            float cube2distance2Axis = Vector3.Distance(_cube2.transform.position, _cubeL.fixedJoint.transform.position);
            Debug.Log("cube2distance2Axis: " + cube2distance2Axis);
            float I_cube2 = _cube2.GetTraegheitsmoment() + _cube2.GetMass() * cube2distance2Axis;
            Debug.Log("I_cube2: " + I_cube2);
            float I_total = I_cubeL + I_cube2;
            Debug.Log("I_total: " + I_total);
            // w = r_1 x m_1 * v_1 / I_total
            float r_1 = Vector3.Distance(_cubeL.GetSchwerpunkt(), _cube2.transform.position);
            Debug.Log("r_1: " + r_1);
            float angleSpeed = r_1 * _cube2.GetMass() * _cube2.GetLastSpeed() / I_total;
            
            
            return angleSpeed;
        }

        public Vector3 CalcAngularMomentum(Rigidbody rb, Vector3 origin) 
        {
            Vector3 R = rb.transform.position - origin;
            Vector3 p = rb.mass * rb.velocity;
            Vector3 L = Vector3.Cross(R, p);
            return L;
        }
        
        public enum CubePos
        {
            Mitte,
            Aussen
        }
        
        private float CalcSumTraegheitsMoment(List<LBodyPart> lBodyParts)
        {
            float sum = 0;
            foreach (LBodyPart lBodyPart in lBodyParts)
            {
                sum += CalcTraegheitsMoment(lBodyPart.rb, lBodyPart.cubePos);
            }

            return sum;
        }
        public float CalcTraegheitsMoment(Rigidbody rb, CubePos cubePos)
        {
            float a = 1; //rb.angularVelocity.magnitude;
            float m = rb.mass;

            switch (cubePos)
            {
                case CubePos.Mitte:
                    return ((1.0f/6.0f) * m * Mathf.Pow(a, 2.0f)) + (m * Mathf.Pow((a/2),2.0f));
                case CubePos.Aussen:
                    return ((1.0f/6.0f) * m * Mathf.Pow(a, 2.0f)) + (m * Mathf.Pow((a/2) + Mathf.Pow(a, 2.0f) ,2.0f));
            }
            
            return 0;
        }

        public override void OnStateExit()
        {
            Sim.WriteProtocol(stateName+ " has Ended");
        }
        
        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            //nah
        }

        private Vector3 GetNewSchwerpunkt()
        {
            return (_cubeL.cube1.transform.position 
                + _cubeL.cube2.transform.position 
                + _cubeL.cube3.transform.position
                + _cube2.transform.position) / 4;
        }
        
        private class LBodyPart
        {
            public Rigidbody rb;
            public CubePos cubePos;
            
            public LBodyPart(Rigidbody rb, CubePos cubePos)
            {
                this.rb = rb;
                this.cubePos = cubePos;
            }
        }
    }
}