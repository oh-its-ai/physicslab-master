using System;
using UnityEngine;
namespace Lab
{
    [CreateAssetMenu(fileName = "Phase3_", menuName = "Phasen/Phase3", order = 1)]
    public class LabStatePhase3 : LabStatePhase2
    {
        public override void OnStateEnter()
        {
            Sim.SetWorldSpeed(stateWorldSpeed);
            Sim.WriteProtocol(stateName + " has Started");
        }

        public override void StateUpdate()
        {
            // Calc Cube1 Forces
            Vector3 cube1FWind = Wind.GetWindForce(Cube1, Vector3.zero, 0f);
            Vector3 cube1Gravity = Cube1.GetMass() * Physics.gravity;
            Vector3 cube1Normal = Cube1.GetNormalForceVector3(0f);
            Vector3 cube1FFriction = Vector3.zero; //Sim.cube1.GetFriction(); not required

            // Calc Cube2 Forces
            Vector3 cube2FWind = Wind.GetWindForce(Cube2, Vector3.zero, 0f);
            Vector3 cube2Gravity = Cube2.GetMass() * Physics.gravity;
            Vector3 cube2Normal = Cube2.GetNormalForceVector3(0f);
            Vector3 cube2FFriction = Vector3.zero; //Sim.cube2.GetFriction(); not required
            
            // Calc Total Forces
            Vector3 cube1FTotal = (cube1FWind - cube1FFriction) + (cube1Gravity - cube1Normal);
            Vector3 cube2FTotal = (cube2FWind - cube2FFriction) + (cube2Gravity - cube2Normal);
            
            // Apply Forces
            Cube1.GetRidgidBody().AddForce(cube1FTotal);
            Cube2.GetRidgidBody().AddForce(cube2FTotal);
            
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