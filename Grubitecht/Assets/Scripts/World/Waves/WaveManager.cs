/*****************************************************************************
// File Name : WaveManager.cs
// Author : Brandon Koederitz
// Creation Date : March 27, 2025
//
// Brief Description : Controls tracking enemies and coordinating when waves spawn and the delay between them.
*****************************************************************************/
using Grubitecht.World;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Waves
{
    public class WaveManager : MonoBehaviour
    {
        private static WaveManager currentLevel;

        public static event Action<int> StartWaveEvent;
        private static int waveNum;

        /// <summary>
        /// Assign and de-assign the wave manager for the current level when the object awakes and is destroyed.
        /// </summary>
        private void Awake()
        {
            if (currentLevel != null && currentLevel != this)
            {
                Debug.LogError("Duplicate WaveManager found.");
                return;
            }
            else
            {
                currentLevel = this;
            }
        }
        private void OnDestroy()
        {
            if (currentLevel == this)
            {
                currentLevel = null;
            }
        }

        public static void LogFinishedWave()
        {

        }
    }
}