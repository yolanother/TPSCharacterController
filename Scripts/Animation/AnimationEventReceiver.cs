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

        public void OnTaggedEvent(string value)
        {
            string tag = "";
            var split = value.Split(new char[] {';'}, 2);
            var name = split[0];
            var type = (AnimationTagType) int.Parse(split[1].TrimEnd(new char[] {';'}));
            if (split.Length > 2)
            {
                tag = split[2];
            }
            onTaggedAnimationEvent.Invoke(type, name, tag);
        }

        public static void AddTaggedEvent(AnimationClip clip, string name, AnimationTag tag)
        {
            AnimationEvent evt = new AnimationEvent();
            evt.time = tag.time;
            evt.functionName = "OnTaggedEvent";
            evt.stringParameter = name + ";" + ((int) tag.tagType) + ";" + tag.tag;

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
            evt.time = Mathf.Max(clip.length - 1, 0);
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
