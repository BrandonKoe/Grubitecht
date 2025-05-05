/*****************************************************************************
// File Name : GameSettings.cs
// Author : Brandon Koederitz
// Creation Date : May 3, 2025
//
// Brief Description : Controls the settings that affect gameplay.
*****************************************************************************/
using Grubitecht.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht
{
    public static class GameSettings
    {
        // Screen Settings
        public static int ResolutionIndex { get; set; }
        public static bool IsFullscreen { get; set; } = true;

        // Audio Settings
        public static float MasterVolume { get; set; } = 0f;
        public static float MusicVolume { get; set; } = 0f;
        public static float SFXVolume { get; set; } = 0f;

        // Gameplay Settings
        private static bool pauseMode = false;
        //public static bool PauseMode { get; set; } = false;
        public static bool PauseMode
        {
            get
            {
                return pauseMode;
            }
            set
            {
                pauseMode = value;
                // If the level menu is already open, then we should immediatly check for changes to the pause state.
                if (LevelMenu.IsOpen)
                {
                    PauseModeController.CheckPauseState();
                }
            }
        }

    }
}