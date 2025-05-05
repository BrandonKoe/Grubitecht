/*****************************************************************************
// File Name : MusicManager.cs
// Author : Brandon Koederitz
// Creation Date : May 5, 2025
//
// Brief Description : Controls music that plays throughout the game.
*****************************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Grubitecht.Audio
{
    public delegate void AudioFadeCallback(Sound sound);
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private float transitionDuration;
        [SerializeField] private Sound[] musicTracks;

        private Sound currentTrack;

        private static MusicManager instance;

        /// <summary>
        /// Assign singleton reference.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                SetupAudioSources();
                // Updates the music for this scene.
                SceneManager.activeSceneChanged += UpdateMusic;
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                SceneManager.activeSceneChanged -= UpdateMusic;
            }
        }

        /// <summary>
        /// Creates an ausio source for each
        /// </summary>
        private void SetupAudioSources()
        {
            foreach (var track in musicTracks)
            {
                track.Setup(gameObject.AddComponent<AudioSource>());
            }
        }

        /// <summary>
        /// Updates the current music that is playing.
        /// </summary>
        /// <param name="current">The current scene that was left.</param>
        /// <param name="next">The new scene that is being loaded.</param>
        private void UpdateMusic(Scene current, Scene next)
        {
            Sound musicForThisScene = Array.Find(musicTracks, item => item.Name == next.name);
            // If we find a new track to play...
            if (musicForThisScene != null)
            {
                // Transition to the new track.
                TransitionToTrack(currentTrack, musicForThisScene);
            }
        }

        /// <summary>
        /// Transitions from the current music track to a new one.
        /// </summary>
        /// <param name="current">The current sound to transition away from.</param>
        /// <param name="next">The next track to transition to.</param>
        private void TransitionToTrack(Sound current, Sound next)
        {
            Debug.Log("Transitioning");
            if (current != null)
            {
                // Fades out the current music.  The reset callback will reset it's volume back to the default after
                // the fade happens.
                StartCoroutine(FadeAudio(current, transitionDuration, 0f, ResetVolume));
            }

            // Plays the new audio and reduces it's volume to 0 so it can be faded in.
            next.Source.Play();
            float targetVolume = next.Volume;
            next.Source.volume = 0f;
            // Fade the new audio in to it's normal volume.
            StartCoroutine(FadeAudio(next, transitionDuration, targetVolume, null));
            // Our next track is now our current track.
            currentTrack = next;
        }

        /// <summary>
        /// Resets the volume of a sound clip.
        /// </summary>
        /// <param name="toReset">The sound clip to reset.</param>
        private void ResetVolume(Sound toReset)
        {
            Debug.Log("reset volume");
            // Stops the audio source.
            toReset.Source.Stop();
            toReset.Source.volume = toReset.Volume;
        }

        /// <summary>
        /// Fades a sound from it's current volume value to a new volume value.
        /// </summary>
        /// <param name="sound">The sound to fade.</param>
        /// <param name="duration">The duration of the fade.</param>
        /// <param name="targetVolume">The target volume that the sound should fade to.</param>
        /// <param name="callback">A callback to call when the fade finishes.</param>
        /// <returns>Coroutine</returns>
        private static IEnumerator FadeAudio(Sound sound, float duration, float targetVolume, 
            AudioFadeCallback callback)
        {
            float timer = duration;
            float startingVolume = sound.Source.volume;
            float normalizedProgress;
            while (timer > 0f)
            {
                normalizedProgress = 1 - (timer / duration);
                sound.Source.volume = Mathf.Lerp(startingVolume, targetVolume, normalizedProgress);

                timer -= Time.unscaledDeltaTime; // Should not scale with timeScale.
                yield return null;
            }
            // Call the callback once we've finished fading.
            callback?.Invoke(sound);
        }
    }
}