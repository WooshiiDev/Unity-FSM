using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FSM
{
    public enum FieldType
    {
        //Built-in types
        INT = 0,
        FLOAT = 1,
        STRING = 2,
        BOOLEAN = 3,

        //Structs
        VECTOR2 = 4,
        VECTOR3 = 5,

        LAYERMASK = 6,

        //Extras
        UNITY = 7,
        OBJECT = 8,


        INVALID = -1,
    }

    [Serializable]
    public class StateFieldInfo
    {
        public string name;
        public Type type;
        public FieldInfo info;

        public FieldType fieldType = FieldType.INVALID;

        // --- Value ---
        public int intValue;
        public float floatValue;
        public bool boolValue;
        public string stringValue;

        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public LayerMask layerMaskValue;

        public UnityEngine.Object unityObjectValue;
        public object objectValue;

        public StateFieldInfo(FieldInfo info)
        {
            this.info = info;
            this.name = info.Name;
            this.type = info.FieldType;

            this.fieldType = GetFieldType ();
        }

        /// <summary>
        /// Update the field information. Normally used if the field type is changed
        /// </summary>
        /// <param name="_info">The reflected field</param>
        /// <param name="_type">The type of field</param>
        public void UpdateInfo(FieldInfo _info, Type _type)
        {
            info = _info;
            type = _type;
            fieldType = GetFieldType ();
        }

        public void SetValue(object instance)
        {
            switch (fieldType)
            {
                case FieldType.INT:
                    info.SetValue (instance, intValue);
                    break;
                case FieldType.FLOAT:
                    info.SetValue (instance, floatValue);
                    break;
                case FieldType.STRING:
                    info.SetValue (instance, stringValue);
                    break;
                case FieldType.BOOLEAN:
                    info.SetValue (instance, boolValue);
                    break;
                case FieldType.VECTOR3:
                    info.SetValue (instance, vector3Value);
                    break;
                case FieldType.VECTOR2:
                    info.SetValue (instance, vector2Value);
                    break;
                case FieldType.UNITY:
                    if (unityObjectValue == null)
                    {
                        return;
                    }

                    info.SetValue (instance, unityObjectValue);
                    break;
                case FieldType.OBJECT:
                    info.SetValue (instance, objectValue);
                    break;

            }
        }

        public FieldType GetFieldType()
        {
            if (type == typeof (int))
            {
                return FieldType.INT;
            }
            else
            if (type == typeof (float))
            {
                return FieldType.FLOAT;
            }
            else
             if (type == typeof (string))
            {
                return FieldType.STRING;
            }
            else
            if (type == typeof (bool))
            {
                return FieldType.BOOLEAN;
            }
            else
            if (type == typeof (Vector2))
            {
                return FieldType.VECTOR2;
            }
            else
            if (type == typeof (Vector3))
            {
                return FieldType.VECTOR3;
            }
            else
            if (type == typeof (LayerMask))
            {
                return FieldType.LAYERMASK;
            }
            else
            if (type.IsSubclassOf (typeof (UnityEngine.Object)))
            {
                return FieldType.UNITY;
            }

            return FieldType.OBJECT;
        }
    }

    [Serializable]
    public class StateInfo
    {
        public string name;

        public Type stateType;
        public List<StateFieldInfo> fields = new List<StateFieldInfo> ();

        public StateInfo(Type type)
        {
            stateType = type;
            name = type.Name;
        }
    }

    [CreateAssetMenu ()]
    public class StateManager : ScriptableObject, ISerializationCallbackReceiver
    {
        // Reflection Flags
        private const BindingFlags REFLECTION_FLAGS = BindingFlags.Public | BindingFlags.Instance;

        // Reflection Collections
        public static Dictionary<Type, StateInfo> RegisteredStates;
        public List<StateInfo> states = new List<StateInfo> ();

        // Get all types
        public static Type[] StateTypes;

        #region Serialization

        public void OnBeforeSerialize()
        {
            OnSerialize ();
        }

        public void OnAfterDeserialize()
        {
            OnSerialize ();
        }

        private void OnSerialize()
        {
            //Get state types
            if (StateTypes == null)
            {
                StateTypes = UnityReflectionUtil.GetTypesInAssembly (typeof (State));
            }

            //Return if no states exist
            if (StateTypes == null || StateTypes.Length == 0)
            {
                Debug.LogWarning ("Cannot find any state types in the project.");
                return;
            }
            else
            if (RegisteredStates == null)
            {
                Initialize ();
            }
        }

        #endregion

        #region Data Setup

        /// <summary>
        /// Initalize or reset the state information
        /// All data will be lost using this, use only when needed
        /// </summary>
        public void Initialize()
        {
            RegisteredStates = new Dictionary<Type, StateInfo> ();

            //Iterate over states and add
            for (int i = 0; i < StateTypes.Length; i++)
            {
                Type stateType = StateTypes[i];
                StateInfo state = states.Find (s => s.name == stateType.Name);

                if (state == null)
                {
                    state = new StateInfo (stateType);
                    states.Add (state);
                }
                else
                {
                    state.stateType = stateType;
                }


                CreateStateFields (state);
                RegisteredStates.Add (stateType, state);
            }

            // Clean up
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].stateType == null)
                {
                    states.RemoveAt (i);
                    i--;
                }
            }
        }

        /// <summary>
        /// OnAwake call for runtime instances to make sure the reflection happens at least once at runtime
        /// </summary>
        public void OnAwake()
        {
            if (StateTypes == null)
            {
                OnSerialize ();
            }
        }

        /// <summary>
        /// Create all fields for the state given
        /// </summary>
        /// <param name="state">Current state we require fields for</param>
        private void CreateStateFields(StateInfo state)
        {
            FieldInfo[] info = state.stateType.GetFields (REFLECTION_FLAGS);
            List<StateFieldInfo> stateFields = state.fields;

            //Check for invalid variables that do not exist anymore
            for (int i = 0; i < stateFields.Count; i++)
            {
                StateFieldInfo field = stateFields[i];
                bool exists = false;

                for (int j = 0; j < info.Length; j++)
                {
                    if (field.name == info[j].Name)
                    {
                        exists = true;
                        continue;
                    }
                }

                //If it's an invalid variable, decrement the loop and remove
                if (!exists)
                {
                    stateFields.Remove (field);
                    i--;
                }
            }

            //Get all fields
            for (int i = 0; i < info.Length; i++)
            {
                FieldInfo field = info[i];

                bool exists = false;
                Type fieldType = field.FieldType;

                // Check if the field already exists
                for (int j = 0; j < stateFields.Count; j++)
                {
                    StateFieldInfo stateField = stateFields[j];

                    if (stateField.name == field.Name)
                    {
                        stateField.UpdateInfo (field, fieldType);
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    continue;
                }

                //Add to state fields
                stateFields.Add (new StateFieldInfo (field));
            }

        }

        #endregion

        #region Runtime/Editor Updating

        /// <summary>
        /// Assign the state values from the manager
        /// </summary>
        /// <param name="currentState">The state to assign values to</param>
        public static void SetStateValues(State currentState)
        {
            if (currentState == null)
            {
                Debug.Log ("Cannot set defaults to a state that is null!");
                return;
            }

            Type type = currentState.GetType ();

            if (!RegisteredStates.ContainsKey (type))
            {
                Debug.Log ("Cannot find state type of " + type);
                return;
            }

            List<StateFieldInfo> fields = RegisteredStates[type].fields;

            for (int i = 0; i < fields.Count; i++)
            {
                fields[i].SetValue (currentState);
            }
        }

        #endregion
    }
}
