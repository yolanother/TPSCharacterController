using System.Xml;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Stats.BaseTypes
{
    public class IntStat : SerializableStat
    {
        [SerializeField] protected int value;

        public static implicit operator int(IntStat stat) => stat.value;
        protected override void OnSave(XmlWriter writer)
        {
            WriteAttribute(name, value);
        }

        protected override void OnLoad(XmlReader reader)
        {
            value = ReadIntAttribute(name, value);
        }
    }
}