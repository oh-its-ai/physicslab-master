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
            Sim.SetWorldSpeed(stateWorldSpeed);
            Sim.SetActiveSpring(false);
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
            
            // todo
            // teil 3 für ims LAB
        }

        
        public override void OnStateExit()
        {
            Sim.WriteProtocol(stateName+ " has Ended");
        }
        
        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            Debug.Log("Cube"+ cube.name + " attached to target"+ target.name);
            // attach cube to target
            CubeController cube2 = target.GetComponent<CubeController>();
            Sim.WriteProtocol("Energie: " + cube2.GetKineticEnergy());
            //cube2.AttachTo(cube.gameObject);
            
            //cube2.DisableRigidbody();
            //cube2.SetKinematic(true);
            Sim.ChangeState();
        }
    }
}