using System;
using UnityEngine;

namespace Lab
{
    public abstract class LabState : ScriptableObject
    {
        public String stateName;
        public LabState nextState;

        protected SimulationController Sim => SimulationController.Instance;
        public abstract void OnStateEnter();
        public abstract void StateUpdate();
        public abstract void OnStateExit();
    }
}