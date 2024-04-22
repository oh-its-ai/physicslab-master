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
            Vector3 cube1FWind = Vector3.zero;
            Vector3 cube1FWindResistance = Wind.GetWindResistanceForce(Sim.cube1, 0);
            Vector3 cube1Gravity = Sim.cube1.GetMass() * Physics.gravity;
            Vector3 cube1Normal = Sim.cube1.GetNormalForceVector3(0f);
            
            Vector3 cube2FWind = Vector3.zero;
            Vector3 cube2FWindResistance = Wind.GetWindResistanceForce(Sim.cube2,0);
            Vector3 cube2Gravity = Sim.cube2.GetMass() * Physics.gravity;
            Vector3 cube2Normal = Sim.cube2.GetNormalForceVector3(0f);
            
            Vector3 cube1FTotal = (cube1FWind - cube1FWindResistance) + (cube1Gravity - cube1Normal);
            Vector3 cube2FTotal = (cube2FWind - cube2FWindResistance) + (cube2Gravity - cube2Normal);
            
            Sim.cube1.GetRidgidBody().AddForce(cube1FTotal);
            Sim.cube2.GetRidgidBody().AddForce(cube2FTotal);
            
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