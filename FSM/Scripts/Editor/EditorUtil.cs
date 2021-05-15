using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FSM.Editors
    {
    /// <summary>
    /// Utility methods for the editor
    /// </summary>
    public static class EditorUtil
        {
        public static Object GetObjectFromGUID(string guid)
            {
            string path = AssetDatabase.GUIDToAssetPath (guid);
            return AssetDatabase.LoadAssetAtPath (path, typeof (Object));
            }

        public static Object GetAssetFromName(string name)
            {
            //Get file/class, gotta make sure that file is named the same
            name = name.Split ('.').Last ();

            //Get GUID for path
            var a = AssetDatabase.FindAssets (name);

            if (a.Length == 0)
            {
                return null;
            }

            string b = AssetDatabase.GUIDToAssetPath (a[0]);

            Object o = null;

            for (int i = 0; i < a.Length; i++)
                {
                o = AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath (a[i]), typeof (Object));

                if (o.name == name)
                    return o;
                }

            return null;
            }

        public static Object GetAssetFromName(string name, Type type)
            {
            //Get file/class, gotta make sure that file is named the same
            name = name.Split ('.').Last ();

            var a = AssetDatabase.FindAssets (name);
            var b = AssetDatabase.GUIDToAssetPath (a[0]);

            return AssetDatabase.LoadAssetAtPath (b, type);
            }

        public static void ClassObjectField(string className)
            {
            //Get file/class, gotta make sure that file is named the same
            className = className.Split ('.').Last ();

            //Get GUID for path
            var a = AssetDatabase.FindAssets (className);
            var b = AssetDatabase.GUIDToAssetPath (a[0]);

            EditorGUILayout.BeginHorizontal ();
                {
                className = char.ToUpper (className[0]) + className.Substring (1);
                EditorGUILayout.LabelField (className + " Script", GUILayout.ExpandWidth(false));
                EditorGUILayout.ObjectField (AssetDatabase.LoadAssetAtPath (b, typeof (Object)), typeof (Object), false);
                }
            EditorGUILayout.EndHorizontal ();
            }

        public static void ClassObjectField(string className, bool showName)
            {
            //Get file/class, gotta make sure that file is named the same
            className = className.Split ('.').Last ();

            //Get GUID for path
            var a = AssetDatabase.FindAssets (className);
            string b = AssetDatabase.GUIDToAssetPath (a[0]);

            Object o = null;

            for (int i = 0; i < a.Length; i++)
                {
                o = AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath (a[i]), typeof (Object));

                if (o.name == className)
                    break;
                }

            if (showName)
                {
                className = char.ToUpper (className[0]) + className.Substring (1);

                EditorGUILayout.BeginHorizontal ();
                    {
                    EditorGUILayout.LabelField (className + " Script", GUILayout.ExpandWidth (false));
                    EditorGUILayout.ObjectField (o, typeof (Object), false);
                    }
                EditorGUILayout.EndHorizontal ();
                }
             else
                {
                EditorGUILayout.ObjectField (o, typeof (Object), false);
                }
            }

        public static void EditorClassField(string className)
            {
            //Get file/class, gotta make sure that file is named the same
            className = className.Split ('.').Last ();

            //Get GUID for path
            var a = AssetDatabase.FindAssets (className);
            var b = AssetDatabase.GUIDToAssetPath (a[0]);

            className = char.ToUpper (className[0]) + className.Substring (1);
            EditorGUILayout.LabelField (className + " Script");
            EditorGUILayout.ObjectField (AssetDatabase.LoadAssetAtPath (b, typeof (UnityEngine.Object)), typeof (UnityEngine.Object), false);
            }
        }
    }
