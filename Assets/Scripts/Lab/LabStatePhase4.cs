using System;
using UnityEngine;
namespace Lab
{
    [CreateAssetMenu(fileName = "Phase4_", menuName = "Phasen/Phase4", order = 1)]
    public class LabStatePhase4 : LabState
    {
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName + " has Started");
            Sim.SwitchToLab3Camera();
            Sim.SetActiveSpring(false);
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
        
        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            // attach cube to target
            cube.AttachTo(target);
            cube.DisableRigidbody();
            Sim.ChangeState();
        }
    }
}