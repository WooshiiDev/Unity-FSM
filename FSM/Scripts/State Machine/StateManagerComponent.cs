using UnityEngine;

namespace FSM
{
    public class StateManagerComponent : MonoBehaviour
    {
        public StateManager manager;

        private void Awake()
        {
            manager.OnAwake ();
        }
    }
}
