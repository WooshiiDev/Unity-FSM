using UnityEngine;

namespace FSM
    {
    public class EntityStateMachineComponent : MonoBehaviour
        {
        public EntityStateManager manager;

        private void Awake()
            {
            manager.OnAwake ();
            }
        }
    }
