using System;
using System.Collections.Generic;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Scripts.Editor.Utilities
{
    public class Dropdown<T>
    {
        private class DropdownItem
        {
            public bool isExpanded;
            public string name;
            public T data;
        }

        List<DropdownItem> items = new List<DropdownItem>();
        private int selectedIndex;
        private string[] names;

        public T Selected
        {
            get => items[selectedIndex].data;
            set
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].data.Equals(value))
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }
        }

        public bool Remove(T data)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].data.Equals(data))
                {
                    items.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void Add(string name, T data)
        {
            items.Add(new DropdownItem()
            {
                name = name,
                data = data
            });
            RegenerateNames();
        }

        private void RegenerateNames()
        {
            names = new string[items.Count];
            for(int i = 0; i < items.Count; i++)
            {
                names[i] = items[i].name;
            }
        }

        public bool Draw(string label)
        {
            
            var index = EditorGUILayout.Popup(label, selectedIndex, names);
            if (selectedIndex != index)
            {
                selectedIndex = index;
                return true;
            }

            return false;
        }
    }
}