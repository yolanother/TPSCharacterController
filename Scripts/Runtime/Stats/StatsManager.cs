using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DoubTech.TPSCharacterController.Stats
{
    public class StatsManager : MonoBehaviour
    {
        [SerializeField] private SerializableStat[] stats;

        private Dictionary<string, SerializableStat> statMap = new Dictionary<string, SerializableStat>();

        private void Awake()
        {
            foreach (var stat in stats)
            {
                statMap[stat.name] = stat;
            }
        }

        public void Save(Stream stream)
        {
            XmlWriter writer = XmlWriter.Create(stream);
            foreach (var stat in stats)
            {
                stat.Save(writer);
            }
        }

        public void Load(Stream stream)
        {
            XmlReader reader = XmlReader.Create(stream);
            foreach (var stat in stats)
            {
                stat.Load(reader);
            }
        }
    }
}
