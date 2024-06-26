using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

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
    public class LabStatePhase0 : LabStatePhaseBase
    {
        public float springLength;
        public float springConstant;
        private SpringController SpringStart => Sim.springStart;
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Sim.SetWorldSpeed(stateWorldSpeed);
            Sim.WriteProtocol(stateName+ " has Started");
        }

        public override void StateUpdate()
        {
            
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
            
            base.OnStateEnter();
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

        public override void LogUpdate()
        {
            base.LogUpdate();
            GetLogValues().AddValue("Cube1_Speed", Cube1.GetSpeed());
            GetLogValues().AddValue("Cube1_Impuls", Cube1.GetImpuls());
            GetLogValues().AddValue("Cube1_KineticEnergy", Cube1.GetKineticEnergy());
            
            GetLogValues().AddValue("Cube2_Speed", Cube2.GetSpeed());
            GetLogValues().AddValue("Cube2_Impuls", Cube2.GetImpuls());
            GetLogValues().AddValue("Cube2_KineticEnergy", Cube2.GetKineticEnergy());
            GetLogValues().AddValue("Spring2_Energy", Sim.spring1.GetSpringForce());
        }
    }
}