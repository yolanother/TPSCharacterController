﻿using System;
using System.Collections.Generic;
 using UnityEditor;
using Object = UnityEngine.Object;

namespace DoubTech.TPSCharacterController.Scripts.Editor.Utilities
{
    public class Foldout<T>
    {
        public class FoldoutItem
        {
            public bool isExpanded;
            public string name;
            public T data;
            public Action<T> onDraw;
            public Action<T> onHide;
            public Action<T> onShow;
            public Action<T> onBeginDrawFoldout;
            public Action<T> onEndDrawFoldout;
        }

        private Dictionary<string, FoldoutItem> foldouts = new Dictionary<string, FoldoutItem>();
        private Dictionary<T, FoldoutItem> foldoutsByData = new Dictionary<T, FoldoutItem>();

        public bool exclusive = false;

        public FoldoutItem Add(string name, T data, Action<T> drawCallback, Action<T> onHide = null, Action<T> onBeginDrawFoldout = null, Action<T> onEndDrawFoldout = null, Action<T> onShow = null)
        {
            var foldout = new FoldoutItem()
            {
                name = name,
                data = data,
                onDraw = drawCallback,
                onShow = onShow,
                onHide = onHide,
                onBeginDrawFoldout = onBeginDrawFoldout, 
                onEndDrawFoldout = onEndDrawFoldout
            };
            foldouts[name] = foldout;
            foldoutsByData[data] = foldout;
            return foldout;
        }

        public void Draw()
        {
            foreach (var foldout in foldouts.Values)
            {
                Draw(foldout);
            }
        }

        public bool Draw(string name)
        {
            if (foldouts.TryGetValue(name, out var foldout))
            {
                return Draw(foldout);
            }

            return false;
        }

        public bool Draw(T data)
        {
            if (foldoutsByData.TryGetValue(data, out var foldout))
            {
                return Draw(foldout);
            }

            return false;
        }

        private bool Draw(FoldoutItem foldout)
        {
            if(null != foldout.onBeginDrawFoldout) foldout.onBeginDrawFoldout.Invoke(foldout.data);
            var expanded = EditorGUILayout.Foldout(
                foldout.isExpanded,
                foldout.name
            );
            if(null != foldout.onEndDrawFoldout) foldout.onEndDrawFoldout.Invoke(foldout.data);

            if (expanded != foldout.isExpanded)
            {
                Expand(foldout, expanded);
            }

            if (foldout.isExpanded)
            {
                foldout.onDraw?.Invoke(foldout.data);
            }

            return foldout.isExpanded;
        }

        private void Expand(FoldoutItem foldout, bool expanded)
        {
            if (expanded != foldout.isExpanded)
            {
                if (expanded)
                {
                    if (exclusive)
                    {
                        foreach (var f in foldouts.Values)
                        {
                            if (f != foldout)
                            {
                                Expand(f, false);
                            }
                        }
                    }
                    foldout.isExpanded = expanded;
                    
                    foldout.onShow?.Invoke(foldout.data);
                }
                else
                {
                    foldout.isExpanded = false;
                    foldout.onHide?.Invoke(foldout.data);
                }
            }
        }

        public void Expand(string name)
        {
            if (foldouts.TryGetValue(name, out var i))
            {
                i.isExpanded = true;
            }
        }

        public void HideAll()
        {
            foreach (var foldout in foldouts.Values)
            {
                foldout.isExpanded = false;
            }
        }

        public bool this[T data]
        {
            get => foldoutsByData.TryGetValue(data, out var result) ? result.isExpanded : false;
        }

        public FoldoutItem this[string name]
        {
            get => foldouts.TryGetValue(name, out var result) ? result : null;
        }
    }
    
    public class Foldout
    {
        private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

        public void SetExpanded(string name, bool expanded)
        {
            foldouts[name] = expanded;
        }

        public bool IsExpanded(string name)
        {
            if (foldouts.TryGetValue(name, out var expanded))
            {
                foldouts[name] = false;
            }
            
            return foldouts[name];
        }
        
        public bool this[string name]
        {
            get
            {
                if (foldouts.TryGetValue(name, out var expanded))
                {
                    foldouts[name] = false;
                }

                foldouts[name] = expanded = EditorGUILayout.Foldout(expanded, name, true);
                return expanded;
            }
            set
            {
                foldouts[name] = value;
            }
        }
    }
}