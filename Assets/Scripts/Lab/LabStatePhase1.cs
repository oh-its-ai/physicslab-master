using System;
using System.Collections.Generic;
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
    [CreateAssetMenu(fileName = "Phase1_", menuName = "Phasen/Phase1", order = 1)]
    public class LabStatePhase1 : LabStatePhase0
    {
        public Vector3 windDirection = new Vector3(1, 0, 0);
        public float windSpeed = 1f;
        
        public override void OnStateEnter()
        {
            Sim.SetWorldSpeed(stateWorldSpeed);
            Sim.WriteProtocol(stateName+ " has Started");
            WindController.Instance.SetWind(windDirection, windSpeed);
            WindController.Instance.EventStartWind();
            WindController.Instance.SetWindForce(windDirection * windSpeed);
        }

        public override void StateUpdate()
        {
            // Calc Cube1 Forces
            Vector3 cube1FWind = Wind.GetWindForce(Cube1, windDirection, windSpeed);
            Vector3 cube1Gravity = Cube1.GetMass() * Physics.gravity;
            Vector3 cube1Normal = Cube1.GetNormalForceVector3(0f);
            Vector3 cube1FFriction = Vector3.zero; //Sim.cube1.GetFriction(); not required

            // Calc Cube2 Forces
            Vector3 cube2FWind = Wind.GetWindForce(Cube2, windDirection, 0f);
            Vector3 cube2Gravity = Cube2.GetMass() * Physics.gravity;
            Vector3 cube2Normal = Cube2.GetNormalForceVector3(0f);
            Vector3 cube2FFriction = Vector3.zero; //Sim.cube2.GetFriction(); not required
            
            // Calc Total Forces
            Vector3 cube1FTotal = (cube1FWind - cube1FFriction) + (cube1Gravity - cube1Normal);
            Vector3 cube2FTotal = (cube2FWind - cube2FFriction) + (cube2Gravity - cube2Normal);
            
            // Apply Forces
            Cube1.GetRidgidBody().AddForce(cube1FTotal);
            Cube2.GetRidgidBody().AddForce(cube2FTotal);
            
            
            // registers if cube1 and the spring1 have collided
            if (Sim.GetCubesDistance() <= Sim.GetActiveLabConfig().springLength)
            {
                WindController.Instance.EventStopWind();
                WindController.Instance.SetWindForce(0f * windDirection);

                Sim.WriteProtocol("Impuls on impact: " + Sim.cube1.GetMass() * Sim.cube1.GetSpeed());
                Sim.ChangeState();
            }
        }

        public override void LogUpdate()
        {
            base.LogUpdate();
            GetLogValues().AddValue("WindSpeed", windSpeed);
        }

        public override void OnStateExit()
        {
            WindController.Instance.EventStopWind();
            Sim.WriteValues(Sim.cube1.GetCubeDataText() + " -> on spring impact");

            Sim.WriteProtocol(stateName+ " has Ended");
        }
        
        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            //nah
        }
    }
}