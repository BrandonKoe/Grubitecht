/*****************************************************************************
// File Name : SettingsMenu.cs
// Author : Brandon Koederitz
// Creation Date : May 3, 2025
//
// Brief Description : Controls the settings menu and allows it to modify with game settings.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace Grubitecht.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        #region CONSTS
        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const string MUSIC_KEY = "MusicVolume";
        private const string SFX_KEY = "SFXVolume";
        #endregion
        [Header("External References")]
        [SerializeField] private AudioMixer audioMixer;
        [Header("Settings References")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        private Resolution[] resolutions;


        #region Resolution
        /// <summary>
        /// Gets the possible resolutions of the user's screen and sets them in the resolution dropdown.
        /// </summary>
        private void SetupResolutions()
        {
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<string> resNames = new List<string>();

            foreach (var resolution in resolutions)
            {
                string resName = $"{resolution.width} x {resolution.height}";
                resNames.Add(resName);
            }

            resolutionDropdown.AddOptions(resNames);
        }

        /// <summary>
        /// Sets the player's current resolution.
        /// </summary>
        /// <param name="resIndex">The index of the item on the resolution dropdown the player selected.</param>
        public void SetResolution(int resIndex)
        {
            if (resolutions.Length < resIndex)
            {
                Resolution res = resolutions[resIndex];

                Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            }
        }
        #endregion

        /// <summary>
        /// Sets if the game is in full screen.
        /// </summary>
        /// <param name="isFullscreen">If the game should be in full screen.</param>
        public void SetFullScreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        #region Volume Settings
        /// <summary>
        /// Sets the volume of various types of sounds based on the function that gets run.
        /// </summary>
        /// <param name="value">The value to set as the volume value of that sound.</param>
        public void SetMaster(float value)
        {
            audioMixer.SetFloat(MASTER_VOLUME_KEY, value);
        }
        public void SetMusic(float value)
        {
            audioMixer.SetFloat(MUSIC_KEY, value);
        }
        public void SetSFX(float value)
        {
            audioMixer.SetFloat(SFX_KEY, value);
        }
        #endregion

        #region Gameplay Settings
        /// <summary>
        /// Sets if gameplay pauses when the player pushes the space bar/opens the pause menu.
        /// </summary>
        /// <param name="value">The value that pause mode should be set at.</param>
        public void SetPauseMode(bool value)
        {
            GameSettings.PauseMode = value;
        }
        #endregion
    }
}