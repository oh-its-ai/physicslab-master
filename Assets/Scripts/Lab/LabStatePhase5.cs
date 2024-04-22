using System;
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
    public class LabStatePhase5 : LabState
    {
        private CubeLController _cubeL;
        private CubeController _cube2;
        
        private float _traegheitsmomentCube2;
        private float _traegheitsmomentCubeL;
        private float _angleSpeed = 0;
        public override void OnStateEnter()
        {
            Sim.SetUpdateGraphs(false);
            _cubeL = Sim.cubeL;
            _cube2 = Sim.cube2;
            //Sim.WriteProtocol(stateName + " has Started");
            // trägheit von Würfel 2 
            _traegheitsmomentCube2 = _cube2.GetTraegheitsmoment();
            //und Würfel L berechnen
            _traegheitsmomentCubeL = _cubeL.GetTraegheitsmoment();
            // I = I_cm + m * r^2
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
            Debug.Log("Winkelgeschwindigkeit: " + angleSpeed);
            _angleSpeed = angleSpeed;
            Sim.WriteProtocol("Winkelgeschwindigkeit: " + angleSpeed);
        }

        public override void StateUpdate()
        {
            Vector3 cube1FWind = Vector3.zero;
            Vector3 cube1FWindResistance = Wind.GetWindResistanceForce(Sim.cube1);
            Vector3 cube1Gravity = Sim.cube1.GetMass() * Physics.gravity;
            Vector3 cube1Normal = Sim.cube1.GetNormalForceVector3(0f);
            
            Vector3 cube2FWind = Vector3.zero;
            Vector3 cube2FWindResistance = Wind.GetWindResistanceForce(Sim.cube2);
            Vector3 cube2Gravity = Sim.cube2.GetMass() * Physics.gravity;
            Vector3 cube2Normal = Sim.cube2.GetNormalForceVector3(0f);
            
            Vector3 cube1FTotal = (cube1FWind - cube1FWindResistance) + (cube1Gravity - cube1Normal);
            Vector3 cube2FTotal = (cube2FWind - cube2FWindResistance) + (cube2Gravity - cube2Normal);
            Sim.cube1.GetRidgidBody().AddForce(cube1FTotal);
            Sim.cube2.GetRidgidBody().AddForce(cube2FTotal);
            
            // apply rotation to cubeL
            _cubeL.RotateAroundFixedJoint(_angleSpeed);
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
    }
}