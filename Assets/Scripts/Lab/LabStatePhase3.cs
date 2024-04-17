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
            // todo
            // teil 3 für ims LAB
        }

        public override void OnStateExit()
        {
            Sim.WriteProtocol(stateName+ " has Ended");
        }
    }
}