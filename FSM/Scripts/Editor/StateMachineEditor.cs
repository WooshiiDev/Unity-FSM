using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#pragma warning disable

namespace FSM.Editors
{
    [CustomEditor (typeof (StateMachine))]
    public class StateMachineEditor : Editor
    {
        public struct StateName
        {
            public readonly string fullName;
            public readonly string displayName;

            public StateName(string fullName, string displayName)
            {
                this.fullName = fullName;
                this.displayName = displayName;
            }
        }

        // State Collections

        /// <summary>
        /// Groups of states 
        /// </summary>
        private static Dictionary<string, List<StateName>> DisplayGroups;

        /// <summary>
        /// The names of the groups
        /// </summary>
        private static string[] GroupNames;

        /// <summary>
        /// The full name of the state types
        /// </summary>
        private static string[] StateNames;

        // Target
        private StateMachine t;

        // Cache selection data for easier references
        private int stateIndex;
        private string selectedState;

        private int groupIndex;
        private string selectedGroup;

        private string[] displayedStates;

        // GUI
        private bool toggleCurrent;
        private bool togglePrevious;

        private bool showDebug = false;

        private void OnEnable()
        {
            t = target as StateMachine;

            SetupCategories ();

            // If the machine has no state assigned (normally on creation, assign the default
            // if one exists (normally just state)
            if (string.IsNullOrEmpty (t.SerializedState.m_stateName))
            {
                groupIndex = 0;
                stateIndex = 0;
            }
            else
            {
                GetSelectionIndexes ();
            }

            UpdateDrawnStates ();
            UpdateSerializedState ();
        }

        private void SetupCategories()
        {
            if (DisplayGroups != null && DisplayGroups.Count > 0)
            {
                return;
            }

            // Add all states to list for lookup
            Type[] assemblyTypes = UnityReflectionUtil.GetTypesInAssembly (typeof (State));
            List<string> groupsTypes = new List<string> ();

            // We only want to check once as they'll update when Unity recompiles
            DisplayGroups = new Dictionary<string, List<StateName>> ();

            // Create cached arrays to have quick references
            StateNames = new string[assemblyTypes.Length];

            // Get each state
            for (int i = 0; i < assemblyTypes.Length; i++)
            {
                // Get state data
                string stateName = assemblyTypes[i].FullName;
                string displayName = GetStateName (stateName);

                string group = GetGroupName (stateName);

                StateName name = new StateName (stateName, displayName);
                if (DisplayGroups.ContainsKey (group))
                {
                    DisplayGroups[group].Add (name);
                }
                else
                {
                    DisplayGroups.Add (group, new List<StateName> () { name });
                }

                StateNames[i] = stateName;
                groupsTypes.Add (group);
            }

            GroupNames = groupsTypes.ToArray ();
        }

        // GUI
        public override void OnInspectorGUI()
        {
            SetupCategories ();

            // Options
            t.isRunning = EditorGUILayout.Toggle ("Is Running", t.isRunning);

            EditorGUILayout.BeginVertical (EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField ("State Selection", EditorStyles.boldLabel);

                string currentState = string.Format ("Current State - {0}", t.SerializedState.m_stateName ?? "No Selected State");
                EditorGUILayout.LabelField (currentState);

                DrawCategories ();
                DrawStates (displayedStates);

                EditorGUILayout.Space ();

                DrawRuntimeInfo ();
            }
            EditorGUILayout.EndVertical ();

            if (serializedObject.UpdateIfRequiredOrScript ())
            {
                Repaint ();
            }
        }

        private void DrawCategories()
        {
            EditorGUI.BeginChangeCheck ();
            {
                groupIndex = EditorGUILayout.Popup ("State Catergory", groupIndex, GroupNames);
            }
            if (EditorGUI.EndChangeCheck ())
            {
                UpdateDrawnStates ();
            }
        }

        private void DrawStates(string[] states)
        {
            if (states == null)
            {
                EditorGUILayout.HelpBox ("Category has no states", MessageType.Error);
                return;
            }

            EditorGUI.BeginChangeCheck ();

            //int xCount = Mathf.Min (_states.Length, 3);

            stateIndex = GUILayout.SelectionGrid (stateIndex, states, 3);

            if (EditorGUI.EndChangeCheck())
            {
                UpdateSerializedState ();
            }
        }

        private void DrawRuntimeInfo()
        {
            EditorGUILayout.LabelField ("Runtime State Info", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            string currentStateStatus = t.CurrentState == null ? "Null" : t.CurrentState.ToString ();
            string previousStateStatus = t.PreviousState == null ? "Null" : t.PreviousState.ToString ();

            string currentStateLabel = string.Format ("Current State ({0})", currentStateStatus);
            string previousStateLabel = string.Format ("Previous State ({0})", previousStateStatus);

            EditorGUI.BeginDisabledGroup (true);

            DrawStateData (currentStateLabel, t.CurrentState, ref toggleCurrent);
            DrawStateData (previousStateLabel, t.PreviousState, ref togglePrevious);

            EditorGUI.EndDisabledGroup ();

            EditorGUI.indentLevel--;
        }

        // State GUI

        private void DrawStateData(string label, State state, ref bool toggle)
        {
            bool isNull = state == null;

            string stateName = isNull ? "" : GetStateName(state.ToString());
            float age = isNull ? 0f : state.age;
            float fixedAge = isNull ? 0f : state.age;

            toggle = EditorGUILayout.Foldout (toggle, label);

            if (toggle)
            {
                EditorGUILayout.TextField ("State Name", stateName);

                EditorGUILayout.FloatField ("Age", age);
                EditorGUILayout.FloatField ("Fixed", fixedAge);
            }
        }

        private void UpdateDrawnStates()
        {
            selectedGroup = GroupNames[groupIndex];
            displayedStates = DisplayGroups[selectedGroup].Select(t => t.displayName).ToArray ();
        }

        private void UpdateSerializedState()
        {
            t.SerializedState.SetStateName (GetSelectedState ());
        }

        // Serialized State

        private string GetSelectedState()
        {
            selectedState = displayedStates[stateIndex];
            return StateNames.FirstOrDefault (state => state.Contains (selectedState));
        }

        private void GetSelectionIndexes()
        {
            string serializedName = t.SerializedState.m_stateName;

            int i = 0;
            foreach (KeyValuePair<string, List<StateName>> group in DisplayGroups)
            {
                int index = group.Value.FindIndex (name => name.fullName == serializedName);

                if (index != -1)
                {
                    stateIndex = index;
                    groupIndex = i;
                    return;
                }

                i++;
            }
        }

        // Helpers

        private string GetStateName(string stateName)
        {
            int nameIndex = stateName.LastIndexOf ('.');

            stateName = stateName.Substring (nameIndex + 1, stateName.Length - nameIndex - 1);

            // Slight clean up
            if (!stateName.Equals("State"))
            {
                stateName = stateName.Replace ("State", "");
            }

            return stateName;
        }

        private string GetGroupName(string stateName)
        {
            int nameIndex = stateName.LastIndexOf ('.');

            string category = stateName.Substring (0, nameIndex);
            int categoryIndex = category.LastIndexOf ('.');

            return stateName.Substring (categoryIndex + 1, category.Length - categoryIndex - 1);
        }
    }
}