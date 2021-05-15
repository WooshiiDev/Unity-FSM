using System;

namespace FSM
{
    [Serializable]
    public class SerializableState
    {
        public string m_stateName;
        public Type m_stateType;

        public State CreateStateFromType()
        {
            if (string.IsNullOrEmpty(m_stateName))
            {
                return new State ();
            }

            m_stateType = Type.GetType (m_stateName);

            if (m_stateType == null)
            {
                return null;
            }

            return (State)Activator.CreateInstance (m_stateType);
        }

        /// <summary>
        /// Set the name of the state to be assigned. Should only really be needed from the editor
        /// </summary>
        /// <param name="name">The full name of the state</param>
        public void SetStateName(string name)
        {
            m_stateName = name;
        }
    }
}



