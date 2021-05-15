using System.Collections.Generic;
using UnityEngine;

namespace FSM.Editors
    {
    public class EditorCategory
        {
        public string catergoryName;

        public bool isFocused = true;
        public int selected;

        public List<string> elements;

        public EditorCategory(string name)
            {
            catergoryName = name;
            elements = new List<string> ();
            }

        public void Add(string element)
            {
            if (string.IsNullOrEmpty (element))
                {
                Debug.Log ("Cannot add element of null to catergory " + catergoryName);
                return;
                }

            elements.Add (element);
            }

        public void Add(string[] elements)
            {
            if (elements == null)
                {
                Debug.Log ("Cannot add null array elements to " + catergoryName);
                return;
                }

            if (elements.Length == 0)
                return;

            for (int i = 0; i < elements.Length; i++)
                Add(elements[i]);
            }

        public string[] LookupNames(string name)
            {
            if (string.IsNullOrEmpty (name))
                return null;

            List<string> names = new List<string> ();

            foreach (var item in elements)
                {
                if (item.Contains (name))
                    names.Add (item);
                }

            return names.ToArray ();
            }

        public string LookUpName(string name)
            {
            if (string.IsNullOrEmpty (name))
                return null;

            foreach (string element in elements)
                {
                if (element == name)
                    return element;
                }

            return null;
            }
        }
    }
