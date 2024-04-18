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
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName+ " has Started");
        }

        public override void StateUpdate()
        {
            if (Sim.GetCubesDistance() > Sim.GetActiveLabConfig().springLength)
            {
                Sim.ChangeState();
            }
        }

        public override void OnStateExit()
        {
            Sim.WriteProtocol(stateName+ " has Ended");
            Sim.NextCamera();
        }

        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            //nah
        }
    }
}