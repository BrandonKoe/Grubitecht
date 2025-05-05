/*****************************************************************************
// File Name : SettingsMenu.cs
// Author : Brandon Koederitz
// Creation Date : May 3, 2025
//
// Brief Description : Controls the settings menu and allows it to modify with game settings.
*****************************************************************************/
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Grubitecht.UI
{
    [RequireComponent(typeof(SubMenu))]
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
        [Header("Sreen Settings")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [Header("Sound Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [Header("Gameplay Settings")]
        [SerializeField] private Toggle pauseModeToggle;

        private Resolution[] resolutions;

        #region ComponentReferences
        [SerializeReference, HideInInspector] private SubMenu subMenu;
        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            subMenu = GetComponent<SubMenu>();
        }
        #endregion
        /// <summary>
        /// Subscribes the UpdateSettingsObjects to this sub menu's OnLoad event because settings should always
        /// accurately reflect the current settings when this menu is loaded.
        /// </summary>

        private void Awake()
        {
            subMenu.OnLoadMenu += UpdateSettingsObjects;
            SetupResolutions();
        }
        private void OnDestroy()
        {
            subMenu.OnLoadMenu -= UpdateSettingsObjects;
        }

        /// <summary>
        /// Updates the settings objects to accurately reflect the values of the current settings.
        /// </summary>
        /// <remarks>
        /// Note that this will cause the Set functions to be called again.  This is inconvenient but ultimately
        /// shouldn't cause problems as long as these values dont get set by said setter functions.
        /// </remarks>
        private void UpdateSettingsObjects()
        {
            // Screen Settings
            resolutionDropdown.value = GameSettings.ResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            fullscreenToggle.isOn = GameSettings.IsFullscreen;

            // Volume Settings.
            masterVolumeSlider.value = GameSettings.MasterVolume;
            musicVolumeSlider.value = GameSettings.MusicVolume;
            sfxVolumeSlider.value = GameSettings.SFXVolume;

            // Game Settings
            pauseModeToggle.isOn = GameSettings.PauseMode;
        }

        #region Resolution
        /// <summary>
        /// Gets the possible resolutions of the user's screen and sets them in the resolution dropdown.
        /// </summary>
        private void SetupResolutions()
        {
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<string> resNames = new List<string>();

            int currentResolution = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                Resolution resolution = resolutions[i];
                string resName = $"{resolution.width} x {resolution.height}";
                resNames.Add(resName);

                // Checks if any of the resoultions match the current resolution of the screen, and set that as the
                // current active resolution.
                if (resolution.width == Screen.currentResolution.width && 
                    resolution.height == Screen.currentResolution.height)
                {
                    currentResolution = i;
                }
            }

            resolutionDropdown.AddOptions(resNames);

            // Set our current resolution after we've added all the options.
            resolutionDropdown.value = currentResolution;
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Sets the player's current resolution.
        /// </summary>
        /// <param name="resIndex">The index of the item on the resolution dropdown the player selected.</param>
        public void SetResolution(int resIndex)
        {
            if (resIndex < resolutions.Length)
            {
                GameSettings.ResolutionIndex = resIndex;
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
            GameSettings.IsFullscreen = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }

        #region Volume Settings
        /// <summary>
        /// Sets the volume of various types of sounds based on the function that gets run.
        /// </summary>
        /// <param name="value">The value to set as the volume value of that sound.</param>
        public void SetMaster(float value)
        {
            GameSettings.MasterVolume = value;
            audioMixer.SetFloat(MASTER_VOLUME_KEY, value);
        }
        public void SetMusic(float value)
        {
            GameSettings.MusicVolume = value;
            audioMixer.SetFloat(MUSIC_KEY, value);
        }
        public void SetSFX(float value)
        {
            GameSettings.SFXVolume = value;
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
            GameSettings.PauseMode = value;
        }
        #endregion
    }
}