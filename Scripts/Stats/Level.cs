using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace DoubTech.TPSCharacterController.Stats
{
    public class Level : SerializableStat
    {
        [SerializeField] private Experience experience;
        [SerializeField] private int level;

        public int CurrentLevel
        {
            get
            {
                if (experience)
                {
                    return (int) Mathf.Log10(Mathf.Max(1f, experience.XP));
                }

                return level;
            }
        }

        protected override void OnSave(XmlWriter writer)
        {
            if (!experience)
            {
                WriteAttribute("level", level);
            }
        }

        protected override void OnLoad(XmlReader reader)
        {
            if (!experience)
            {
                level = ReadIntAttribute("level", 1);
            }
        }
    }
}
