using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Utils
{
    public class FoldoutHierarchy<T>
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

        private SortedDictionary<string, FoldoutHierarchy<T>>
            children = new SortedDictionary<string, FoldoutHierarchy<T>>();

        private SortedDictionary<string, Item> items = new SortedDictionary<string, Item>();
        private string name;
        private bool isFoldout;
        private bool isExpanded;

        public FoldoutHierarchy(string name = null, bool isFoldout = true)
        {
            this.name = name;
            this.isFoldout = isFoldout;
            if (!isFoldout) isExpanded = true;
        }

        internal void Clear()
        {
            children.Clear();
            items.Clear();
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
                    children[path[index]] = new FoldoutHierarchy<T>(path[index]);
                }

                children[path[index]].Add(item, path, index + 1);
            }
        }

        public void Draw()
        {
            if (isFoldout)
            {
                isExpanded = EditorGUILayout.Foldout(isExpanded, name, true);
            }
            else if (null != name)
            {
                EditorGUILayout.LabelField(name);
            }

            if (isExpanded)
            {
                EditorGUILayout.BeginHorizontal();
                if (null != name)
                {
                    GUILayout.Space(16);
                }

                EditorGUILayout.BeginVertical();
                foreach (FoldoutHierarchy<T> foldout in children.Values)
                {
                    foldout.Draw();
                }

                foreach (Item item in items.Values)
                {
                    item.onDraw(item);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
