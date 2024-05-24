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
        private Rigidbody _rbCubeLCube1;
        private Rigidbody _rbCubeLCube2;
        private Rigidbody _rbCubeLCube3;
        private Vector3 _cube2HitImpuls;

        private Vector3 _drehImpuls;
        
        private List<LBodyPart> _lBodyParts = new List<LBodyPart>();
        private Vector3 _cube2HitDir;

        public override void OnStateEnter()
        {
            Sim.SetWorldSpeed(stateWorldSpeed);
            Sim.SetUpdateGraphs(false);
            
            Sim.NextCamera(2);
            
            _rbCubeLCube1 = CubeL.cube1.GetComponent<Rigidbody>();
            _rbCubeLCube2 = CubeL.cube2.GetComponent<Rigidbody>();
            _rbCubeLCube3 = CubeL.cube3.GetComponent<Rigidbody>();
            
            _lBodyParts.Add(new LBodyPart(_rbCubeLCube1, CubePos.Mitte));
            _lBodyParts.Add(new LBodyPart(_rbCubeLCube2, CubePos.Aussen));
            _lBodyParts.Add(new LBodyPart(_rbCubeLCube3, CubePos.Mitte));
            _lBodyParts.Add(new LBodyPart(Cube2.GetRidgidBody(), CubePos.Aussen));
            
            // register hit
            Cube2.AddToJoint(JointCubeL);
            Cube2.DisableCollider();
        }

        public override void StateUpdate()
        {
            float sumTgm = CalcSumTraegheitsMoment(_lBodyParts);
            _drehImpuls = (sumTgm) * Cube2.GetRidgidBody().angularVelocity;
            
            Debug.Log("Bahndrehimpuls: " + BahnDrehImpuls + " : Mag: " + BahnDrehImpuls.magnitude);
            Debug.Log("L EigenDrehimpuls: V3: " + _drehImpuls + " : Mag: " + _drehImpuls.magnitude);
        }
        
        public override void LogUpdate()
        {
            base.LogUpdate();
            GetLogValues().AddValue("L_EigenDrehImpuls", _drehImpuls.magnitude);
            GetLogValues().AddValue("L_GesamtDrehImpuls", BahnDrehImpuls.magnitude + _drehImpuls.magnitude);
        }
        
        public float CalcAngleSpeed()
        {
            float I_cubeL = CubeL.GetTraegheitsmoment() + CubeL.GetMass()* CubeL.GetDistanceCubeToAxis();
            Debug.Log("I_cubeL: " + I_cubeL);
            float cube2distance2Axis = Vector3.Distance(Cube2.transform.position, CubeL.fixedJoint.transform.position);
            Debug.Log("cube2distance2Axis: " + cube2distance2Axis);
            float I_cube2 = Cube2.GetTraegheitsmoment() + Cube2.GetMass() * cube2distance2Axis;
            Debug.Log("I_cube2: " + I_cube2);
            float I_total = I_cubeL + I_cube2;
            Debug.Log("I_total: " + I_total);
            // w = r_1 x m_1 * v_1 / I_total
            float r_1 = Vector3.Distance(CubeL.GetSchwerpunkt(), Cube2.transform.position);
            Debug.Log("r_1: " + r_1);
            float angleSpeed = r_1 * Cube2.GetMass() * Cube2.GetLastSpeed() / I_total;
            
            
            return angleSpeed;
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
            float a = 1f; 
            float m = rb.mass;
            switch (cubePos)
            {
                case CubePos.Mitte:
                    return ((1.0f/6.0f) * m * Mathf.Pow(a, 2.0f)) + (m * Mathf.Pow((a/2.0f),2.0f));
                case CubePos.Aussen:
                    return ((1.0f/6.0f) * m * Mathf.Pow(a, 2.0f)) + (m * (Mathf.Pow(a, 2.0f) + Mathf.Pow((a/2.0f), 2.0f)));
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