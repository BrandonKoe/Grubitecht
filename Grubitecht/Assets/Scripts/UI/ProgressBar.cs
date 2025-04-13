/*****************************************************************************
// File Name : ProgressBar.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Controls the UI progress bar that denotes the number of enemies left.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Grubitecht.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image progressBarImage;
        private int enemyNum;
        /// <summary>
        /// Starts handling a given wave with a certain number of enemies.
        /// </summary>
        /// <param name="enemyNum">The number of enemies in the wave.</param>
        public void StartWave(int enemyNum)
        {
            gameObject.SetActive(true);
            this.enemyNum = enemyNum;
            UpdateProgressBar(enemyNum);
        }

        /// <summary>
        /// Stops the progress bar once the wave is finished.
        /// </summary>
        public void EndWave()
        {
            gameObject.SetActive(false);
            enemyNum = 0;
        }

        /// <summary>
        /// Updates this progress bar to accurately reflect the number of enemies left in a wave.
        /// </summary>
        /// <param name="currentEnemies">The number of enemies left in the wave.</param>
        public void UpdateProgressBar(int currentEnemies)
        {
            float normalizedProgress = currentEnemies / enemyNum;
            progressBarImage.fillAmount = normalizedProgress;
        }
    }
}