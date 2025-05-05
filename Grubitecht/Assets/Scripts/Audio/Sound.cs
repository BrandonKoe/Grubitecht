/*****************************************************************************
// File Name : Sound.cs
// Author : Brandon Koederitz
// Creation Date : May 5, 2025
//
// Brief Description : Set of information that acts as settings for a particular sound.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Audio;

namespace Grubitecht.Audio
{
    [CreateAssetMenu(fileName = "Sound", menuName = "Grubitecht/Sound")]
    public class Sound : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AudioClip AudioClip { get; private set; }
        [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
        [field: SerializeField, Range(0f, 256f)] public int Priority { get; private set; } = 128;
        [field: SerializeField, Range(0f, 1f)] public float Volume { get; private set; } = 1f;
        [field: SerializeField, Range(-3f, 3f)] public float Pitch { get; private set; } = 1f;
        [field: SerializeField] public bool Loop { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float SpatialBlend { get; private set; } = 0f;
        [field: SerializeField] public float MinDistance { get; private set; } = 1f;
        [field: SerializeField] public float MaxDistance { get; private set; } = 500f;

        //public AudioSource Source { get; set; }

        /// <summary>
        /// Sets up an audio source for this sound.
        /// </summary>
        public void Setup(AudioSource source)
        {
            source.clip = AudioClip;
            source.outputAudioMixerGroup = MixerGroup;
            source.priority = Priority;
            source.volume = Volume;
            source.pitch = Pitch;
            source.loop = Loop;
            source.spatialBlend = SpatialBlend;
            source.minDistance = MinDistance;
            source.maxDistance = MaxDistance;
        }
    }
}