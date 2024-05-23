using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The LabState class is a base class for all LabStates. It is a ScriptableObject and contains the following abstract methods:
 * - OnStateEnter(): Called when the state is entered
 * - StateUpdate(): Called every frame while the state is active
 * - OnStateExit(): Called when the state is exited
 * 
 * The LabState class contains a reference to the SimulationController, which is used in the abstract methods to access the simulation.
 * 
 * The LabState class is used to create different LabStates for the simulation, such as LabStatePhase1, LabStatePhase2, LabStatePhase3, and LabStatePhase4.
 * Each LabState is a ScriptableObject that inherits from LabState and implements the abstract methods.
 * 
 * The LabState class is part of the Lab namespace.
 * 
 */
namespace Lab
{
    public abstract class LabState : ScriptableObject
    {
        public class LogValues
        {
            public List<String> Headers = new List<string>();
            public List<List<float>> Values = new List<List<float>>();
            
            public void AddValue(String header, float value)
            {
                if (Headers.Exists(x => x == header))
                {
                    Values[Headers.IndexOf(header)].Add(value);
                }
                else
                {
                    Headers.Add(header);
                    
                    var newList = new List<float>();
                    //fill with zeros
                    var count = Values.Count > 0 ? Values[0].Count : 0;
                    for (int i = 0; i < count; i++)
                    {
                        newList.Add(0);
                    }
                    newList.Add(value);
                    Values.Add(newList);
                }
            }

            public List<List<float>> GetTransposedValues()
            {
                var transposed = new List<List<float>>();
                for (int i = 0; i < Values[0].Count; i++)
                {
                    var newList = new List<float>();
                    for (int j = 0; j < Values.Count; j++)
                    {
                        newList.Add(Values[j][i]);
                    }
                    transposed.Add(newList);
                }

                return transposed;
            }
            
            public string GetHeaderString()
            {
                return String.Join(",", Headers);
            }
        }
        
        protected Vector3 BahnDrehImpuls
        {
            get => Sim.bahndrehimpuls;
            set => Sim.bahndrehimpuls = value;
        }
        
        public String stateName;
        public LabState nextState;

        public float stateWorldSpeed = 1f;
        protected SimulationController Sim => SimulationController.Instance;
        protected WindController Wind => WindController.Instance;
        
        protected CubeController Cube1 => Sim.cube1;
        protected CubeController Cube2 => Sim.cube2;
        protected CubeLController CubeL => Sim.cubeL;
        protected Rigidbody JointCubeL => Sim.jointCubeL;
        public abstract void OnStateEnter();
        public abstract void StateUpdate();
        
        public abstract void LogUpdate();
        public abstract void OnStateExit();

        protected LogValues LogValuesData
        {
            get => Sim.LogValues;
            set => Sim.LogValues = value;
        }

        public abstract LogValues GetLogValues();
        
        public abstract void RegisterEvent(CubeController cube, GameObject target);
    }
}