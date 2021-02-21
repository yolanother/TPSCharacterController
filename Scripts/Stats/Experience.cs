using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace DoubTech.TPSCharacterController.Stats
{
    public class Experience : SerializableStat
    {
        [SerializeField] private float experience;

        public float XP => experience;
        protected override void OnSave(XmlWriter writer)
        {
            WriteAttribute("experience", experience);
        }

        protected override void OnLoad(XmlReader reader)
        {
            experience = ReadFloatAttribute("experience", experience);
        }
    }
}
