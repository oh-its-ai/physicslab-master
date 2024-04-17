using System;
using UnityEngine;

/*
 * Phase1 is a LabState that represents the first phase of the simulation.
 * It is a ScriptableObject and inherits from LabState.
 * 1. It starts the wind controller.
 * 2. It checks if the cubes have collided.
 * 3. It stops the wind controller.
 * 4. It writes a protocol entry.
 */
namespace Lab
{
    [CreateAssetMenu(fileName = "Phase1_", menuName = "Phasen/Phase1", order = 1)]
    public class LabStatePhase1 : LabState
    {
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName+ " has Started");
            WindController.Instance.EventStartWind();
        }

        public override void StateUpdate()
        {
            if (Sim.GetCubesDistance() <= Sim.GetActiveLabConfig().springLength)
            {
                WindController.Instance.EventStopWind();
                Sim.WriteProtocol("Impuls on impact: " + Sim.cube1.GetMass() * Sim.cube1.GetSpeed());
                Sim.ChangeState();
            }
        }

        public override void OnStateExit()
        {
            WindController.Instance.EventStopWind();
            Sim.WriteProtocol(stateName+ " has Ended");
        }
    }
}