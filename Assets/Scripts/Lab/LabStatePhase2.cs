using System;
using UnityEngine;
namespace Lab
{
    [CreateAssetMenu(fileName = "Config", menuName = "Phasen/Phase2", order = 1)]
    public class LabStatePhase2 : LabState
    {
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName);
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
            // nix
        }
    }
}