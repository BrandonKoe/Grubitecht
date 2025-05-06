/*****************************************************************************
// File Name : MusicManager.cs
// Author : Brandon Koederitz
// Creation Date : May 5, 2025
//
// Brief Description : Controls SFX played by objects in the game.
*****************************************************************************/
using Unity.VisualScripting;
using UnityEngine;

namespace Grubitecht.Audio
{
    public class AudioManager : MonoBehaviour
    {
        //[SerializeField] private AudioMixerGroup mixerGroup;
        //[SerializeField] private Sound[] sounds;

        //private static AudioManager instance;

        //#region Properties
        //private static AudioManager Instance

        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = FindObjectOfType<AudioManager>();
        //        }
        //        return instance;
        //    }
        //}
        //#endregion

        ///// <summary>
        ///// Assign singleton reference.
        ///// </summary>
        //private void Awake()
        //{
        //    if (instance != null && instance != this)
        //    {
        //        Destroy(gameObject);
        //        return;
        //    }
        //    else
        //    {
        //        instance = this;
        //        DontDestroyOnLoad(gameObject);
        //    }
        //}

        //private void OnDestroy()
        //{
        //    if (instance == this)
        //    {
        //        instance = null;
        //    }
        //}

        /// <summary>
        /// Plays a sound with a given name at a given position.
        /// </summary>
        /// <param name="sound">The name of the sound to play.</param>
        /// <param name="position">The position to play the sound at.</param>
        public static void PlaySoundAtPosition(Sound sound, Vector3 position, bool dontDestroyOnLoad = false)
        {
            //Sound sound = Array.Find(Instance.sounds, item => item.Name == sound);
            if (sound == null)
            {
                Debug.Log($"No sound provided");
                return;
            }

            // Creates a game object that will play the sound.
            GameObject soundGo = new GameObject(sound.Name);
            AudioSource source = soundGo.AddComponent<AudioSource>();
            sound.Setup(source);
            // Mark the sound object as DontDestroyOnLoad if that option is set.
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(source.gameObject);
            }
            // Multiply in Time.timeScale as the object shouldn't take longer to be destroyed if the time scale is
            // changed.
            source.Play();
            Destroy(soundGo, sound.AudioClip.length * (Time.timeScale < 0.009f ? 0.01f : Time.timeScale));
        }
    }
}