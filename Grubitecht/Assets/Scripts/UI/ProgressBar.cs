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
        [SerializeField] private RectTransform progressBarCap;
        [SerializeField] private float capOffset = -50f;
        private float currentEnemies;
        private float enemyNum;
        /// <summary>
        /// Starts handling a given wave with a certain number of enemies.
        /// </summary>
        /// <param name="enemyNum">The number of enemies in the wave.</param>
        public void StartWave(int enemyNum)
        {
            gameObject.SetActive(true);
            this.enemyNum = enemyNum;
            this.currentEnemies = enemyNum;
            UpdateProgressBar();
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
        /// Updates this progress bar to reflect a change in enemy numbers.
        /// </summary>
        /// <param name="enemyChange">The change in the number of enemies.</param>
        public void LogEnemyChange(int enemyChange)
        {
            currentEnemies += enemyChange;
            UpdateProgressBar();
        }

        /// <summary>
        /// Updates this progress bar to accurately reflect the number of enemies left in a wave.
        /// </summary>
        private void UpdateProgressBar()
        {
            float normalizedProgress = currentEnemies / enemyNum;
            //Debug.Log(normalizedProgress);
            progressBarImage.fillAmount = normalizedProgress;

            // Moves the image that caps the progress bar.
            RectTransform rectTrans = transform as RectTransform;
            Vector2 anchorPos = progressBarCap.anchoredPosition;
            anchorPos.x = (rectTrans.rect.width * normalizedProgress) + capOffset;
            progressBarCap.anchoredPosition = anchorPos;
        }
    }
}