using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Grubitecht.Audio
{
    [System.Serializable]
    public class Sound
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AudioClip AudioClip { get; private set; }
        [field:SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
        [field: SerializeField, Range(0f, 256f)] public int Priority { get; private set; } = 128;
        [field: SerializeField, Range(0f, 1f)] public float Volume { get; private set; } = 1f;
        [field: SerializeField, Range(-3f, 3f)] public float Pitch { get; private set; } = 1f;
        [field: SerializeField] public bool Loop { get; private set; }

        public AudioSource Source { get; private set; }

        /// <summary>
        /// Sets up an audio source for this sound.
        /// </summary>
        public void Setup(AudioSource source)
        {
            Source = source;
            source.clip = AudioClip;
            source.outputAudioMixerGroup = MixerGroup;
            source.priority = Priority;
            source.volume = Volume;
            source.pitch = Pitch;
            source.loop = Loop;
        }
    }
}