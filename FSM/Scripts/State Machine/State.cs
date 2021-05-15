using System;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// Base State class 
    /// </summary>
    public class State
    {
        // Connection to main GameObject
        protected StateMachine parent;

        //Timers 
        public float age;
        public float fixedAge;

        public State()
        {
            if (Application.isPlaying)
            {
                StateManager.SetStateValues (this);
            }
        }

        /// <summary>
        /// Called when entering the state. Use for getting references or setting up
        /// </summary>
        public virtual void OnEnter()
        {

        }

        /// <summary>
        /// Called when exiting the state.
        /// </summary>
        public virtual void OnExit()
        {

        }

        /// <summary>
        /// Update tick
        /// </summary>
        public virtual void Tick(float delta)
        {
            age += Time.deltaTime;
        }

        /// <summary>
        /// Fixed update tick
        /// </summary>
        public virtual void FixedTick(float delta)
        {
            fixedAge += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Late update tick
        /// </summary>
        public virtual void LateTick(float delta)
        {

        }

        /// <summary>
        /// Debug update tick
        /// </summary>
        public virtual void DebugTick(float delta)
        {

        }

        /// <summary>
        /// Set the parent of this state
        /// </summary>
        /// <param name="parent">The state parent</param>
        public void SetParent(StateMachine parent)
        {
            this.parent = parent;
        }

        public static explicit operator State(Type v)
        {
            throw new NotImplementedException ();
        }
    }
}
