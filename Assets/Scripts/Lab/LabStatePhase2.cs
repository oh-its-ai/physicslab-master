using System;
using UnityEngine;
/*
 * The LabStatePhase2 class is a ScriptableObject that represents the second phase of the lab simulation.
 * Object that represents the second phase of the lab simulation.
 * 1. It starts when the cubes collide on the spring
 * 2. It ends when the cubes are separated by a certain distance.
 */
namespace Lab
{
    [CreateAssetMenu(fileName = "Phase2_", menuName = "Phasen/Phase2", order = 1)]
    public class LabStatePhase2 : LabState
    {
        public float SpringLength => Sim.GetActiveLabConfig().springLength; // m
        public float SpringConstant => Sim.GetActiveLabConfig().springConstant; // N/m
        private float _springCompression;
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName+ " has Started");
            Sim.spring1.SetSpringLength(SpringLength);
            Sim.spring1.SetSpringConstant(SpringConstant);
        }

        public override void StateUpdate()
        {
            // apply wind resistance to the cubes
            WindController.Instance.ApplyWindresistance(Sim.cube1.GetRidgidBody());
            WindController.Instance.ApplyWindresistance(Sim.cube2.GetRidgidBody());
            
            if (Sim.GetCubesDistance() <= (SpringLength))
            {
                Sim.spring1.SetSpringLength(Sim.GetCubesDistance());
            
                _springCompression = SpringLength - Sim.GetCubesDistance();
                Sim.spring1.SetSpringCompression(_springCompression);
                
                float force = SpringConstant * _springCompression;
                float forceCube1 = force/Sim.cube1.GetMass();
                float forceCube2 = force/Sim.cube2.GetMass();
                Sim.cube1.GetRidgidBody().AddForce(new Vector3(-forceCube1,0,0), ForceMode.Acceleration);
                Sim.cube2.GetRidgidBody().AddForce(new Vector3(forceCube2,0,0), ForceMode.Acceleration);
            }
            // registers if cube1 and the spring1 have parted their ways
            if (Sim.GetCubesDistance() > SpringLength)
            {
                Sim.ChangeState();
            }
        }

        public override void OnStateExit()
        {
            Sim.WriteProtocol(stateName+ " has Ended");
            Sim.WriteValues(Sim.cube1.GetCubeDataText() + " -> after spring");
            Sim.WriteValues(Sim.cube2.GetCubeDataText() + " -> after spring");
            Sim.NextCamera(0);
        }

        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            //nah
        }
    }
}