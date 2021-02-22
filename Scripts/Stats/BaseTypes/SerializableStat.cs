using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DoubTech.TPSCharacterController.Stats
{
    public abstract class SerializableStat : MonoBehaviour
    {
        private XmlWriter activeWriter;
        private XmlReader activeReader;
        
        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement(name);
            OnSave(writer);
            writer.WriteEndElement();
        }

        public void Load(XmlReader reader)
        {
            reader.ReadStartElement(name);
            OnLoad(reader);
            reader.ReadEndElement();
        }

        protected void WriteAttribute(string name, float value)
        {
            activeWriter.WriteStartElement(name);
            activeWriter.WriteStartAttribute(name);
            activeWriter.WriteValue(value);
            activeWriter.WriteEndAttribute();
        }

        protected float ReadFloatAttribute(string name, float defaultValue)
        {
            float current = defaultValue;
            if (float.TryParse(activeReader.GetAttribute(name), out var newcurrent)) current = newcurrent;
            return current;
        }

        protected int ReadIntAttribute(string name, int defaultValue)
        {
            int current = defaultValue;
            if (int.TryParse(activeReader.GetAttribute(name), out var newcurrent)) current = newcurrent;
            return current;
        }

        protected abstract void OnSave(XmlWriter writer);
        protected abstract void OnLoad(XmlReader reader); 
    }
}
