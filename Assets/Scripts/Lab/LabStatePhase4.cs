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
            
            Sim.SetActiveSpring(false);
        }

        public override void StateUpdate()
        {
            // apply wind resistance to the cubes
            Vector3 cube1FWind = Vector3.zero;
            Vector3 cube1FWindResistance = Wind.GetWindResistanceForce(Sim.cube1);
            Vector3 cube1Gravity = Sim.cube1.GetMass() * Physics.gravity;
            Vector3 cube1Normal = Sim.cube1.GetNormalForceVector3(0f);
            
            Vector3 cube2FWind = Vector3.zero;
            Vector3 cube2FWindResistance = Wind.GetWindResistanceForce(Sim.cube2);
            Vector3 cube2Gravity = Sim.cube2.GetMass() * Physics.gravity;
            Vector3 cube2Normal = Sim.cube2.GetNormalForceVector3(0f);
            
            Vector3 cube1FTotal = (cube1FWind - cube1FWindResistance) + (cube1Gravity - cube1Normal);
            Vector3 cube2FTotal = (cube2FWind - cube2FWindResistance) + (cube2Gravity - cube2Normal);
            
            Sim.cube1.GetRidgidBody().AddForce(cube1FTotal);
            Sim.cube2.GetRidgidBody().AddForce(cube2FTotal);
            
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

            cube2.AttachTo(cube.gameObject);
            cube2.DisableRigidbody();
            Sim.ChangeState();
        }
    }
}