using System;
using UnityEngine;
namespace Lab
{
    [CreateAssetMenu(fileName = "Config", menuName = "Phasen/Phase3", order = 1)]
    public class LabStatePhase3 : LabState
    {
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName);
        }

        public override void StateUpdate()
        {
            // todo
            // start wind
        }

        public override void OnStateExit()
        {
            // stop wind
            
        }
    }
}