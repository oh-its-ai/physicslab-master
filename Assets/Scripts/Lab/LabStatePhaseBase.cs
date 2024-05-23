using System.Collections.Generic;
using UnityEngine;

namespace Lab
{
    public class LabStatePhaseBase : LabState
    {
        private Vector3 _cube1ForceTotal;
        private Vector3 _cube2ForceTotal;
        
        
        public override void OnStateEnter()
        {
            Sim.SetWorldSpeed(stateWorldSpeed);
        }

        public override void LogUpdate()
        {
            GetLogValues().AddValue("Time", Sim.GetSimTimeInSeconds());
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
            _cube1ForceTotal = (cube1FWind - cube1FFriction) + (cube1Gravity - cube1Normal);
            _cube2ForceTotal = (cube2FWind - cube2FFriction) + (cube2Gravity - cube2Normal);
            
            // Apply Forces
            Cube1.GetRidgidBody().AddForce(_cube1ForceTotal);
            Cube2.GetRidgidBody().AddForce(_cube2ForceTotal);
        }

        public override void OnStateExit()
        {
            //throw new System.NotImplementedException();
        }

        public override LogValues GetLogValues()
        {
            //LogValuesData = new LogValues();
            return LogValuesData;
        }

        public override void RegisterEvent(CubeController cube, GameObject target)
        {
            //
        }
    }
}