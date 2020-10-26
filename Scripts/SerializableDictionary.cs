using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine.InputSystem;

namespace DoubTech.TPSCharacterController
{
    /// <summary>
    /// A dumb serializable dictionary. There are better implementations out there, but this one let me be lazy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SerializableDictionary<T>
    {
        // Stored data
        public List<string> keys = new List<string>();
        public List<T> values = new List<T>();

        private Dictionary<string, T> dictionary;

        private Dictionary<string, T> Dictionary
        {
            get
            {
                if (null == dictionary)
                {
                    dictionary = new Dictionary<string, T>();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        dictionary[keys[i]] = values[i];
                    }
                }

                return dictionary;
            }
        }

        public bool TryGetValue(string key, out T result)
        {
            return Dictionary.TryGetValue(key, out result);
        }
        public bool ContainsKey(string name)
        {
            return Dictionary.ContainsKey(name);
        }

        public T this[string key]
        {
            get => Dictionary[key];
            set
            {
                Dictionary[key] = value;
                Reindex();
            }
        }

        public void Remove(string key)
        {
            Dictionary.Remove(key);
            Reindex();
        }

        private void Reindex()
        {
            keys.Clear();
            values.Clear();
            foreach (var pair in Dictionary)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
    }
}
