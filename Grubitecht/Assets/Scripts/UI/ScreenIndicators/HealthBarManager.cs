/*****************************************************************************
// File Name : HealthBarManager.cs
// Author : Brandon Koederitz
// Creation Date : April 15, 2025
//
// Brief Description : Controls the creation of health bars for objectives.
*****************************************************************************/
using Grubitecht.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.UI
{
    public class HealthBarManager : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBarPrefab;
        private static HealthBarManager instance;

        /// <summary>
        /// Assign the singleton instance that will be used as the parent for health bars.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("Multiple Health Bar Managers exist.");
                return;
            }
            else
            {
                instance = this;
            }
        }
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Creates a new health bar for an attackable that doesn't have one.
        /// </summary>
        /// <param name="owner">The attackable that the health bar is for.</param>
        /// <param name="maxHP">The max hp of the attackable this health bar is for.</param>
        /// <returns>The created health bar.</returns>
        public static HealthBar CreateHealthBar(Attackable owner, int maxHP)
        {
            HealthBar hpBar = Instantiate(instance.healthBarPrefab, instance.transform);
            hpBar.Initialize(owner, maxHP);
            return hpBar;
        }
    }
}