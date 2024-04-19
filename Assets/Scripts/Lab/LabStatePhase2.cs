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
        public float springLength;
        public float springConstant;
        private float _springCompression;
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName+ " has Started");
            springLength = Sim.GetActiveLabConfig().springLength;
            springConstant = Sim.GetActiveLabConfig().springConstant;
        }

        public override void StateUpdate()
        {
            if (Sim.GetCubesDistance() <= (Sim.GetActiveLabConfig().springLength))
            {
                Sim.spring1.SetSpringLength(Sim.GetCubesDistance());
            
                _springCompression = springLength - Sim.GetCubesDistance();
                float force = springConstant * _springCompression;
                Sim.cube1.AddForce(-force);
                Sim.cube2.AddForce(force);
            }
            // registers if cube1 and the spring1 have parted their ways
            if (Sim.GetCubesDistance() > Sim.GetActiveLabConfig().springLength)
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