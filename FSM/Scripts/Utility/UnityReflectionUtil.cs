using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

public static class UnityReflectionUtil
{
    public static void ReflectMethods<T>(BindingFlags flags, bool showParameters)
    {
        string reflectedString = "";

        foreach (MethodInfo method in typeof (T).GetMethods (flags))
        {
            reflectedString = method.Name + "\n";

            if (showParameters)
            {
                ParameterInfo[] parameters = method.GetParameters ();

                if (parameters != null)
                {
                    reflectedString += "(";

                    foreach (ParameterInfo info in parameters)
                    {
                        reflectedString += $"{info.ParameterType} {info.Name}";
                    }

                    reflectedString += ")";
                }
            }

            //Write Debug, then clear string
            Debug.Log (reflectedString);
        }
    }

    public static void ReflectFields<T>()
    {
        foreach (FieldInfo field in typeof (T).GetRuntimeFields ())
        {
            Debug.Log ($"{field.FieldType} {field.Name}");
        }
    }

    #region Field Info

    public static FieldInfo[] GetFields([NotNull] Type type, BindingFlags flags = BindingFlags.Public)
    {
        //Assembly assem = type.Assembly;
        if (type == null)
        {
            Debug.LogError ("Cannot find type of null!");
            return null;
        }

        return type.GetFields (flags);
    }

    #endregion

    #region Method Info

    public static MethodInfo[] GetMethods([NotNull] Type type, BindingFlags flags = BindingFlags.Public)
    {
        //Assembly assem = type.Assembly;
        if (type == null)
        {
            Debug.LogError ("Cannot find type of null!");
            return null;
        }

        return type.GetMethods (flags);
    }

    #endregion

    #region Assemblies

    public static Type[] GetSubclassesInAssembly(Type type)
    {
        Assembly assem = type.Assembly;

        if (assem == null)
        {
            Debug.Log ("Cannot find assembly of " + type + "!");
            return null;
        }

        return assem.GetTypes ().Where (t => t.IsSubclassOf (type)).ToArray ();
    }

    public static Type[] GetTypesInAssembly(Type type)
    {
        Assembly assem = type.Assembly;

        if (assem == null)
        {
            Debug.Log ("Cannot find assembly of " + type + "!");
            return null;
        }

        return assem.GetTypes ().Where (t => t.IsAssignableFrom (type) || t.IsSubclassOf (type)).ToArray ();
    }

    #endregion
}