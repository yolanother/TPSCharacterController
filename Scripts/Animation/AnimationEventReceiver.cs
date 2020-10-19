using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Animation
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        [SerializeField]
        private NamedAnimationEvent onNamedAnimationEvent = new NamedAnimationEvent();
        [SerializeField]
        private NamedAnimationEvent onAnimationStart = new NamedAnimationEvent();
        [SerializeField]
        private NamedAnimationEvent onAnimationEnd = new NamedAnimationEvent();
        [SerializeField]
        private TaggedAnimationEvent onTaggedAnimationEvent = new TaggedAnimationEvent();

        public NamedAnimationEvent OnNamedAnimationEvent => onNamedAnimationEvent;
        public NamedAnimationEvent OnAnimationStart => onAnimationStart;
        public NamedAnimationEvent OnAnimationEnd => onAnimationEnd;
        public TaggedAnimationEvent OnTaggedAnimationEvent => onTaggedAnimationEvent;

        public void SendEvent(string eventName)
        {
            onNamedAnimationEvent.Invoke(eventName);
        }

        public void OnStartAnimation(string name)
        {
            onAnimationStart.Invoke(name);
        }

        public void OnStopAnimation(string name)
        {
            onAnimationEnd.Invoke(name);
        }

        public void OnTaggedEvent(int tagType, string value)
        {
            AnimationTagType type = (AnimationTagType) tagType;
            string name = value;
            string tag = "";
            if (type == AnimationTagType.Custom)
            {
                var split = value.Split(new char[] {';'}, 1);
                name = split[0];
                tag = split[1];
            }
            onTaggedAnimationEvent.Invoke(type, name, tag);
        }

        public static void AddTaggedEvent(AnimationClip clip, string name, AnimationTag tag)
        {
            AnimationEvent evt = new AnimationEvent();
            evt.time = tag.time;
            evt.functionName = "OnTaggedEvent";
            evt.intParameter = (int) tag.tagType;
            if (tag.tagType == AnimationTagType.Custom)
            {
                evt.stringParameter = name + ";" + tag.tag;
            }
            else
            {
                evt.stringParameter = name;
            }

            clip.AddEvent(evt);
        }

        public static void AddNamedEvent(AnimationClip clip, string name,  float time)
        {
            AnimationEvent evt = new AnimationEvent();
            evt.time = time;
            evt.functionName = "SendEvent";
            evt.stringParameter = name;
            clip.AddEvent(evt);
        }

        public static void AddStartAnimationEvent(AnimationClip clip, string name)
        {
            AnimationEvent evt = new AnimationEvent();
            evt.time = 0;
            evt.functionName = "OnStartAnimation";
            evt.stringParameter = name;
            clip.AddEvent(evt);
        }

        public static void AddStopAnimationEvent(AnimationClip clip, string name)
        {
            AnimationEvent evt = new AnimationEvent();
            evt.time = clip.length - 1;
            evt.functionName = "OnStopAnimation";
            evt.stringParameter = name;
            clip.AddEvent(evt);
        }
    }

    [SerializeField]
    public class NamedAnimationEvent : UnityEvent<String>
    {
    }

    [SerializeField]
    public class TaggedAnimationEvent : UnityEvent<AnimationTagType, string, string>
    {
    }
}
