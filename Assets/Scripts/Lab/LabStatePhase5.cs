using System;
using UnityEngine;
namespace Lab
{
    [CreateAssetMenu(fileName = "Phase5_", menuName = "Phasen/Phase5", order = 1)]
    public class LabStatePhase5 : LabState
    {
        private CubeLController _cubeL;
        private CubeController _cube2;
        
        private float _traegheitsmomentCube2;
        private float _traegheitsmomentCubeL;
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName + " has Started");
            // trägheit von Würfel 2 
            _traegheitsmomentCube2 = _cube2.GetTraegheitsmoment();
            //und Würfel L berechnen
            _traegheitsmomentCubeL = _cubeL.GetTraegheitsmoment();
            // I = I_cm + m * r^2
            float I_cubeL = _cubeL.GetTraegheitsmoment() + _cubeL.GetMass()* _cubeL.GetDistanceCubeToAxis();
            float cube2distance2Axis = Vector3.Distance(_cube2.transform.position, _cubeL.fixedJoint.transform.position);
            float I_cube2 = _cube2.GetTraegheitsmoment() + _cube2.GetMass() * cube2distance2Axis;
            float I_total = I_cubeL + I_cube2;
            // w = r_1 x m_1 * v_1 / I_total
            float r_1 = Vector3.Distance(_cubeL.GetSchwerpunkt(), _cube2.transform.position);
            float angleSpeed = r_1 * _cube2.GetMass() * _cube2.GetSpeed() / I_total;
            // apply rotation to cubeL
            
        }

        public override void StateUpdate()
        {
            _cubeL.CalculateAngleSpeed();
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