using System;
using UnityEngine;

/*
 * Phase1 is a LabState that represents the first phase of the simulation.
 * It is a ScriptableObject and inherits from LabState.
 * 1. It starts the wind controller.
 * 2. It checks if the cubes have collided.
 * 3. It stops the wind controller.
 * 4. It writes a protocol entry.
 */
namespace Lab
{
    [CreateAssetMenu(fileName = "Phase0_", menuName = "Phasen/Phase0", order = 1)]
    public class LabStatePhase0 : LabState
    {
        public float springLength;
        public float springConstant;
        private SpringController SpringStart => Sim.springStart;
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName+ " has Started");
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
            
            if(!SpringStart.cubeRight) return;
            if(SpringStart.cubeLeft) return;
            float springCompression = SpringStart.length - SpringStart.GetDistanceToCubeRight();
            float force = SpringStart.springConstant * springCompression;


            if (force < 0)
            {
                Debug.Log("Cube has been yeeted");
                SpringStart.cubeRight = null;
                Sim.ChangeState();
                
            }
            
            if(SpringStart.cubeRight)
                SpringStart.cubeRight.AddForce(SpringStart.IsCubeRightToTheRight() ? force : -force);
        }

        public override void OnStateExit()
        {
            Sim.WriteValues(Sim.cube1.GetCubeDataText() + " -> on release");
            Sim.NextCamera(1);
        }

        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            //nah
        }
    }
}