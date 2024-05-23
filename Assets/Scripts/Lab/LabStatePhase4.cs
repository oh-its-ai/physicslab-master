using System;
using UnityEngine;
namespace Lab
{
    [CreateAssetMenu(fileName = "Phase4_", menuName = "Phasen/Phase4", order = 1)]
    public class LabStatePhase4 : LabStatePhase3
    {
        

        protected Vector3 JointOrigin => CubeL.fixedJoint.transform.position;
        protected Vector3 HitJointPos;
        private bool _collectData = true;

        public override void OnStateEnter()
        {
            Sim.WriteProtocol(stateName + " has Started");
            Sim.SetWorldSpeed(stateWorldSpeed);
            Sim.SetActiveSpring(false);
            HitJointPos = JointOrigin;
            Debug.Log("CCCCCCCCC");
            
                Debug.Log("BBBBBBBBBBB");
                BahnDrehImpuls = CalcAngularMomentum(Cube2.GetRidgidBody(), HitJointPos);
                Debug.Log("AAAAAAAAAAAA");
                _collectData = false;
            
        }

        protected Vector3 CalcAngularMomentum(Rigidbody rb, Vector3 origin) 
        {
            Vector3 R = rb.transform.position - origin;
            Vector3 p = rb.mass * rb.velocity;
            Vector3 L = Vector3.Cross(R, p);
            return L;
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

        public override void LogUpdate()
        {
            base.LogUpdate();
            
            GetLogValues().AddValue("Cube2_BahnDrehImpuls", BahnDrehImpuls.magnitude);
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
            //_collectData = false;
            //cube2.DisableRigidbody();
            //cube2.SetKinematic(true);
            Sim.ChangeState();
        }
    }
}