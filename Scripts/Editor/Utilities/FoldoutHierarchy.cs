using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Utils
{
    public class PopupHierarchy<T>
    {
        public class Item
        {
            public string name;
            public T data;

            public Action<Item> onDraw;

            public Item(string name)
            {
                this.name = name;
            }

            public Item(string name, T data)
            {
                this.name = name;
                this.data = data;
            }

            public override bool Equals(object obj)
            {
                if (obj is Item)
                {
                    Item itm = obj as Item;
                    return itm.name == name;
                }
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return name.GetHashCode();
            }
        }

        private SortedDictionary<string, PopupHierarchy<T>>
            children = new SortedDictionary<string, PopupHierarchy<T>>();

        private SortedDictionary<string, Item> items = new SortedDictionary<string, Item>();
        private string[] names = new string[0];
        private object[] values = new object[0];
        private string name;
        private bool isPopup;
        private bool isExpanded;
        private int index;

        public PopupHierarchy(string name = null, bool isPopup = true)
        {
            this.name = name;
            this.isPopup = isPopup;
            if (!isPopup) isExpanded = true;
        }

        internal void Clear()
        {
            children.Clear();
            items.Clear();
            names = new string[0];
        }

        public void Add(Item item, string[] path, int index = 0)
        {
            if (index >= path.Length) return;

            if (index + 1 == path.Length)
            {
                items[item.name] = item;
            }
            else
            {
                if (!children.ContainsKey(path[index]))
                {
                    children[path[index]] = new PopupHierarchy<T>(path[index]);
                }

                children[path[index]].Add(item, path, index + 1);
            }

            RefreshNames();
        }

        private void RefreshNames()
        {
            names = new string[children.Count + items.Count + 1];
            values = new string[children.Count + items.Count + 1];
            names[0] = "";
            int idx = 1;
            foreach (var child in children)
            {
                values[idx] = child.Value;
                names[idx++] = "> " + child.Key;
            }
            foreach (var i in items.Values)
            {
                values[idx] = i;
                names[idx++] = i.name;
            }
        }

        public void Draw()
        {
            if (isPopup)
            {
                index = EditorGUILayout.Popup(index, names);
            }
            else if (null != name)
            {
                EditorGUILayout.LabelField(name);
            }

            if (index > 0)
            {
                if (values[index] is PopupHierarchy<T>)
                {
                    (values[index] as PopupHierarchy<T>).Draw();
                }
                else
                {
                    var item = values[index] as Item;
                    item.onDraw?.Invoke(item);
                }
            }
        }
    }
}
