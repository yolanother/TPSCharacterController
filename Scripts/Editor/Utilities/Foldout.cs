﻿using System;
using System.Collections.Generic;
 using PhantasmaBob;
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
            public Action<T> onBeginDrawFoldout;
            public Action<T> onEndDrawFoldout;
        }

        private Dictionary<string, FoldoutItem> foldouts = new Dictionary<string, FoldoutItem>();


        public FoldoutItem Add(string name, T data, Action<T> drawCallback, Action<T> onHide = null, Action<T> onBeginDrawFoldout = null, Action<T> onEndDrawFoldout = null)
        {
            var foldout = new FoldoutItem()
            {
                name = name,
                data = data,
                onDraw = drawCallback,
                onHide = onHide,
                onBeginDrawFoldout = onBeginDrawFoldout, 
                onEndDrawFoldout = onEndDrawFoldout
            };
            foldouts[name] = foldout;
            return foldout;
        }

        public void Draw()
        {
            foreach (var foldout in foldouts.Values)
            {
                if(null != foldout.onBeginDrawFoldout) foldout.onBeginDrawFoldout.Invoke(foldout.data);
                var expanded = EditorGUILayout.Foldout(
                    foldout.isExpanded,
                    foldout.name
                );
                if(null != foldout.onEndDrawFoldout) foldout.onEndDrawFoldout.Invoke(foldout.data);

                if (expanded != foldout.isExpanded)
                {
                    foldout.isExpanded = expanded;
                    if(!expanded && null != foldout.onHide) foldout.onHide.Invoke(foldout.data);
                }

                if (foldout.isExpanded)
                {
                    foldout.onDraw.Invoke(foldout.data);
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
    }
    
    public class Foldout
    {
        private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
        
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
        }
    }
}