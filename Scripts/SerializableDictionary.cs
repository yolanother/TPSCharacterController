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
        
        private Dictionary<string, T> dictionary = new Dictionary<string, T>();

        public bool TryGetValue(string key, out T result)
        {
            return dictionary.TryGetValue(key, out result);
        }
        public bool ContainsKey(string name)
        {
            return dictionary.ContainsKey(name);
        }

        public T this[string key]
        {
            get => dictionary[key];
            set
            {
                dictionary[key] = value;
                Reindex();
            }
        }

        public void Remove(string key)
        {
            dictionary.Remove(key);
            Reindex();
        }

        private void Reindex()
        {
            keys.Clear();
            values.Clear();
            foreach (var pair in dictionary)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
    }
}
