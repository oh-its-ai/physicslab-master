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
    [CreateAssetMenu(fileName = "Phase0_", menuName = "Phasen/Phase0", order = 1)]
    public class LabStatePhase0 : LabState
    {
        public Vector3 windDirection = new Vector3(1, 0, 0);
        public float windSpeed = 1f;
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName+ " has Started");
            WindController.Instance.SetWind(windDirection, windSpeed);
            WindController.Instance.EventStopWind();
        }

        public override void StateUpdate()
        {
            
        }

        public override void OnStateExit()
        {
            
        }

        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            //nah
        }
    }
}