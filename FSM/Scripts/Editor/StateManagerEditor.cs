using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FSM.Editors
{
    //TODO: Better lookup for states, catergories are setup painfully bad
    [CustomEditor (typeof (StateManager))]
    public class StateManagerEditor : Editor
    {
        // --- Static Data ---
        private static StateInfo[] states;      //Editor States
        private static string[] stateNames;     //Collection of state names

        // --- GUI State ---
        private static int catergoryIndex;      //Current selected catergory
        private static int stateIndex;          //Current selected state
        private static StateInfo currentState;         //The state itself
        private static Object stateAsset;

        // Property Convenience
        private SerializedProperty stateInfo;
        private SerializedProperty fields;

        private Dictionary<string, EditorCategory> stateCatergories;
        private Dictionary<string, StateInfo> stateCollection;

        // --- Target ---
        private static StateManager Target;

        private void OnEnable()
        {
            Target = target as StateManager;
            Initialize ();
        }

        private void Initialize()
        {
            // Create collections
            stateCatergories = new Dictionary<string, EditorCategory> ();
            stateCollection = new Dictionary<string, StateInfo> ();

            states = Target.states.ToArray ();
            stateNames = new string[states.Length];

            for (int i = 0; i < states.Length; i++)
            {
                StateInfo state = states[i];
                string stateName = stateNames[i] = state.name;

                if (!stateCollection.ContainsKey (stateName))
                {
                    stateCollection.Add (stateName, state);
                }

                string[] stateType = state.stateType.FullName.Split ('.');
                string name = stateType.Length == 1 ? stateType[0] : stateType[stateType.Length - 2];

                if (!stateCatergories.ContainsKey (name))
                {
                    stateCatergories.Add (name, new EditorCategory (name));
                }

                stateCatergories[name].Add (stateType.Last ());

            }

            UpdateSelection ();
        }

        // GUI
        public override void OnInspectorGUI()
        {
            if (states == null)
            {
                GUILayout.Label ("No states to display");
                return;
            }

            DrawSelection ();
            DrawState (currentState);
        }

        private void DrawSelection()
        {
            EditorGUILayout.LabelField ("State Selection", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck ();
            {
                catergoryIndex = EditorGUILayout.Popup ("State Catergory", catergoryIndex, stateCatergories.Keys.ToArray ());

                EditorCategory[] catergories = stateCatergories.Values.ToArray ();

                if (catergories != null && catergories.Length != 0)
                {
                    EditorCategory category = catergories[catergoryIndex];

                    if (category != null && category.elements != null && category.elements.Count != 0)
                    {
                        stateIndex = GUILayout.SelectionGrid (stateIndex, stateCatergories.Values.ToArray ()[catergoryIndex].elements.ToArray (), 3, EditorStyles.toolbarButton);
                    }
                }
            }
            if (EditorGUI.EndChangeCheck ())
            {
                UpdateSelection ();
            }
        }

        private void UpdateSelection()
        {
            GUI.FocusControl (null);

            EditorCategory[] catergories = stateCatergories.Values.ToArray ();

            if (catergories != null && catergories.Length != 0)
            {
                List<string> elements = stateCatergories.ToArray ()[catergoryIndex].Value.elements;

                if (catergories != null || catergories.Length != 0)
                {
                    stateIndex = Mathf.Clamp (stateIndex, 0, elements.Count);
                }

                if (stateIndex >= 0 && stateIndex < elements.Count)
                {
                    currentState = stateCollection[elements[stateIndex]];
                }
                else
                {
                    stateIndex = 0;
                    currentState = stateCollection[elements[0]];
                }

                stateInfo = serializedObject.FindProperty ("states").GetArrayElementAtIndex (stateIndex);
                fields = stateInfo.FindPropertyRelative ("fields");
            }
        }

        // State GUI
        private void DrawState(StateInfo state)
        {
            if (state == null)
            {
                return;
            }

            stateAsset = EditorUtil.GetAssetFromName (state.name);

            if (stateAsset == null)
            {
                stateAsset = EditorUtil.GetAssetFromName ("State");
            }

            EditorGUILayout.Space ();

            GUIStyle skin = new GUIStyle (GUI.skin.window)
            {
                padding = new RectOffset (2, 0, 0, 0),
                margin = new RectOffset (0, 0, 0, 0),
            };

            //Draw all catergories in order for hiearchial type display
            if (stateAsset != null)
            {
                EditorGUILayout.InspectorTitlebar (true, stateAsset, false);
            }

            List<string> names = new List<string> ();

            //Toggle for convenience
            EditorGUI.indentLevel++;
            {
                for (int i = 0; i < state.fields.Count; i++)
                {
                    StateFieldInfo field = state.fields[i];
                    string name = field.info.DeclaringType.Name;

                    // Draw based on what the declaring type is
                    if (!names.Contains (name))
                    {
                        bool isScriptName = i == 0;

                        if (!isScriptName)
                        {
                            EditorGUILayout.Space ();
                        }

                        if (isScriptName)
                        {
                            EditorGUILayout.BeginHorizontal ();
                            {
                                EditorGUILayout.LabelField (name, EditorStyles.boldLabel);
                                EditorGUILayout.ObjectField (stateAsset, typeof (MonoScript), false);
                            }
                            EditorGUILayout.EndHorizontal ();
                        }
                        else
                        {
                            EditorGUILayout.LabelField (name, EditorStyles.boldLabel);
                        }

                        names.Add (name);
                    }

                    DrawField (state.fields[i]);
                }

                EditorGUILayout.Space ();
            }
            EditorGUI.indentLevel--;

            EditorUtility.SetDirty (Target);
        }

        private void DrawField(StateFieldInfo info)
        {
            foreach (object attribute in info.info.GetCustomAttributes (true))
            {
                if (attribute is HeaderAttribute header)
                {
                    EditorGUILayout.LabelField (header.header, EditorStyles.boldLabel);
                }
            }

            switch (info.fieldType)
            {
                case FieldType.INT:
                    info.intValue = EditorGUILayout.DelayedIntField (info.name, info.intValue);
                    break;
                case FieldType.FLOAT:
                    info.floatValue = EditorGUILayout.DelayedFloatField (info.name, info.floatValue);
                    break;
                case FieldType.STRING:
                    info.stringValue = EditorGUILayout.DelayedTextField (info.name, info.stringValue);
                    break;
                case FieldType.BOOLEAN:
                    info.boolValue = EditorGUILayout.Toggle (info.name, info.boolValue);
                    break;
                case FieldType.VECTOR3:
                    info.vector3Value = EditorGUILayout.Vector3Field (info.name, info.vector3Value);
                    break;
                case FieldType.VECTOR2:
                    info.vector2Value = EditorGUILayout.Vector2Field (info.name, info.vector2Value);
                    break;
                case FieldType.UNITY:
                    info.unityObjectValue = EditorGUILayout.ObjectField (info.name, info.unityObjectValue, typeof (Object), false);
                    break;
                case FieldType.LAYERMASK:
                    LayerMask tempMask = EditorGUILayout.MaskField (
                        info.name,
                        InternalEditorUtility.LayerMaskToConcatenatedLayersMask (info.layerMaskValue),
                        InternalEditorUtility.layers);

                    info.layerMaskValue = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask (tempMask);
                    break;

                case FieldType.OBJECT:

                    break;

                case FieldType.INVALID:
                    string invalidFormat = string.Format ("Invalid field type of {0} with the name {1}", info.type, info.name);
                    EditorGUILayout.HelpBox (invalidFormat, MessageType.Warning, true);
                    break;

                default:
                    break;
            }

        }

        // Helpers
        private SerializedProperty GetFieldProperty(int index, StateFieldInfo field)
        {
            SerializedProperty serializedField = fields.GetArrayElementAtIndex (index);

            switch (field.fieldType)
            {
                case FieldType.INT:
                    return serializedField.FindPropertyRelative ("intValue");

                case FieldType.FLOAT:
                    return serializedField.FindPropertyRelative ("floatValue");

                case FieldType.STRING:
                    return serializedField.FindPropertyRelative ("stringValue");

                case FieldType.BOOLEAN:
                    return serializedField.FindPropertyRelative ("boolValue");

                case FieldType.VECTOR2:
                    return serializedField.FindPropertyRelative ("vector2Value");

                case FieldType.VECTOR3:
                    return serializedField.FindPropertyRelative ("vector3Value");

                case FieldType.LAYERMASK:
                    return serializedField.FindPropertyRelative ("layerMaskValue");

                case FieldType.UNITY:
                    return serializedField.FindPropertyRelative ("unityObjectValue");

                case FieldType.OBJECT:
                    return serializedField.FindPropertyRelative ("objectValue");
            }

            return null;
        }
    }
}