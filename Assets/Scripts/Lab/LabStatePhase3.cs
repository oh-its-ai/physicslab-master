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
            // apply wind resistance to the cubes
            WindController.Instance.ApplyWindresistance(Sim.cube1.GetRidgidBody());
            WindController.Instance.ApplyWindresistance(Sim.cube2.GetRidgidBody());
            
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