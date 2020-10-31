using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Scripts.Animation;
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
        [SerializeField]
        private NamedAnimationEvent onPlaySound = new NamedAnimationEvent();
        [SerializeField]
        private StateEnterEvent onEnterState = new StateEnterEvent();
        [SerializeField]
        private StateExitEvent onExitState = new StateExitEvent();

        public StateEnterEvent OnEnterState => onEnterState;
        public StateExitEvent OnExitState => onExitState;
        
        public NamedAnimationEvent OnNamedAnimationEvent => onNamedAnimationEvent;
        public NamedAnimationEvent OnAnimationStart => onAnimationStart;
        public NamedAnimationEvent OnAnimationEnd => onAnimationEnd;
        public TaggedAnimationEvent OnTaggedAnimationEvent => onTaggedAnimationEvent;
        public NamedAnimationEvent OnPlaySound => onPlaySound;

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

        public void OnPlaySoundEvent(string name)
        {
            onPlaySound.Invoke(name);
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
            bool found = false;
            foreach (var e in clip.events)
            {
                if (e.stringParameter == name)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                AnimationEvent evt = new AnimationEvent();
                evt.time = time;
                evt.functionName = "SendEvent";
                evt.stringParameter = name;
                clip.AddEvent(evt);
            }
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

        public static void AddAudioEvent(AnimationClip clip, AnimationSoundTag soundTag)
        {
            AnimationEvent evt = new AnimationEvent();
            evt.time = Mathf.Max(soundTag.time, 0);
            evt.functionName = "OnPlaySoundEvent";
            evt.stringParameter = soundTag.sound.name;
            clip.AddEvent(evt);
        }

        public void StateEnterEvent(string tag)
        {
            onEnterState.Invoke(tag);
        }

        public void StateExitEvent(string tag)
        {
            onExitState.Invoke(tag);
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

    public class StateEnterEvent : UnityEvent<string> { }

    public class StateExitEvent : UnityEvent<string> { }
}
