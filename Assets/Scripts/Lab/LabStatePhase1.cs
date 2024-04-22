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
    [CreateAssetMenu(fileName = "Phase1_", menuName = "Phasen/Phase1", order = 1)]
    public class LabStatePhase1 : LabState
    {
        public Vector3 windDirection = new Vector3(1, 0, 0);
        public float windSpeed = 1f;
        
        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName+ " has Started");
            WindController.Instance.SetWind(windDirection, windSpeed);
            WindController.Instance.EventStartWind();
        }

        public override void StateUpdate()
        {
            Vector3 cube1FWind = windDirection * windSpeed;
            Vector3 cube1FWindResistance = Wind.GetWindResistanceForce(Sim.cube1, windSpeed);
            Vector3 cube1Gravity = Sim.cube1.GetMass() * Physics.gravity;
            Vector3 cube1Normal = Sim.cube1.GetNormalForceVector3(0f);
            
            Vector3 cube2FWind = Vector3.zero;
            Vector3 cube2FWindResistance = Wind.GetWindResistanceForce(Sim.cube2,0);
            Vector3 cube2Gravity = Sim.cube2.GetMass() * Physics.gravity;
            Vector3 cube2Normal = Sim.cube2.GetNormalForceVector3(0f);
            
            Vector3 cube1FTotal = (cube1FWind - cube1FWindResistance) + (cube1Gravity - cube1Normal);
            Vector3 cube2FTotal = (cube2FWind - cube2FWindResistance) + (cube2Gravity - cube2Normal);
            
            Sim.LogForces(cube1FWind, cube1FWindResistance, cube1Gravity, cube1Normal, cube1FTotal
                , cube2FWind, cube2FWindResistance, cube2Gravity, cube2Normal, cube2FTotal);
            
            Sim.cube1.GetRidgidBody().AddForce(cube1FTotal);
            Sim.cube2.GetRidgidBody().AddForce(cube2FTotal);
            
            // apply wind resistance to the cubes
            //WindController.Instance.ApplyWindresistance(Sim.cube1.GetRidgidBody());
            //WindController.Instance.ApplyWindresistance(Sim.cube2.GetRidgidBody());
            
            // registers if cube1 and the spring1 have collided
            if (Sim.GetCubesDistance() <= Sim.GetActiveLabConfig().springLength)
            {
                WindController.Instance.EventStopWind();
                Sim.WriteProtocol("Impuls on impact: " + Sim.cube1.GetMass() * Sim.cube1.GetSpeed());
                Sim.ChangeState();
            }
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