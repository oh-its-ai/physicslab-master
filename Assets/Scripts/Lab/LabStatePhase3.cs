using System;
using UnityEngine;
namespace Lab
{
    [CreateAssetMenu(fileName = "Phase3_", menuName = "Phasen/Phase3", order = 1)]
    public class LabStatePhase3 : LabState
    {
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName + " has Started");
        }

        public override void StateUpdate()
        {
            if (Sim.GetSimTimeInSeconds() >= 7)
            {
                Sim.ChangeState();
            }
        }

        public override void OnStateExit()
        {
            Sim.WriteProtocol(stateName+ " has Ended");
        }

        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            //nah
        }
    }
}