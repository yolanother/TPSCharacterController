using System.Xml;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Stats.BaseTypes
{
    public class FloatStat : SerializableStat
    {
        [SerializeField] protected float value;

        public static implicit operator float(FloatStat stat) => stat.value;
        protected override void OnSave(XmlWriter writer)
        {
            WriteAttribute(name, value);
        }

        protected override void OnLoad(XmlReader reader)
        {
            value = ReadFloatAttribute(name, value);
        }
    }
}