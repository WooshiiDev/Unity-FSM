using UnityEngine;

namespace FSM
{
    public class StateMachine : MonoBehaviour
    {
        public bool isRunning = true;

        private State m_mainState;

        [SerializeField] private State m_currentState;
        [SerializeField] private State m_previousState;

        [SerializeField] private SerializableState m_serializedMainState = new SerializableState();

        public State CurrentState => m_currentState;
        public State PreviousState => m_previousState;

        public SerializableState SerializedState => m_serializedMainState;

        // Unity Methods

        private void OnEnable()
        {
            m_mainState = m_serializedMainState.CreateStateFromType ();
            SetState (m_mainState);
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            if (m_currentState != null)
            {
                m_currentState.Tick (Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (!isRunning)
            {
                return;
            }

            if (m_currentState != null)
            {
                m_currentState.FixedTick (Time.fixedDeltaTime);
            }
        }

        private void LateUpdate()
        {
            if (!isRunning)
            {
                return;
            }

            if (m_currentState != null)
            {
                m_currentState.LateTick (Time.deltaTime);
            }
        }

        private void OnDrawGizmos()
        {
            if (!isRunning)
            {
                return;
            }

            if (m_currentState != null)
            {
                m_currentState.DebugTick (Time.deltaTime);
            }
        }

        // State Methods

        /// <summary>
        /// Set a new state to the instance
        /// </summary>
        /// <param name="state"></param>
        public void SetState(State state)
        {
            if (!isRunning)
            {
                return;
            }

            if (state == null)
            {
                Debug.LogError ("Cannot set null state");
                return;
            }

            if (m_currentState != null)
            {
                m_currentState.OnExit ();
            }

            m_previousState = m_currentState;
            
            // Update State
            m_currentState = state;
            m_currentState.SetParent(this);
            m_currentState.OnEnter ();
        }

        /// <summary>
        /// Revert the current state to the default state assigned to this instance
        /// </summary>
        public void SetToDefault()
        {
            SetState (m_mainState);
        }
    }
}

