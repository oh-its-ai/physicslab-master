using System;
using UnityEngine;
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
        }
    }
}