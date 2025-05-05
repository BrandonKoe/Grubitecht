/*****************************************************************************
// File Name : HealthBar.cs
// Author : Brandon Koederitz
// Creation Date : April 15, 2025
//
// Brief Description : Controls health bars that show the current health percentage of the objectives.
*****************************************************************************/
using Grubitecht.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Grubitecht.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Vector2 offset;

        private Attackable owner;
        private float maxHP;

        /// <summary>
        /// Initializes this HP bar on creation.
        /// </summary>
        /// <param name="owner">The attackable that this healp bar represents.</param>
        /// <param name="maxHP">The max HP of the attackable this health bar represents.</param>
        public void Initialize(Attackable owner, int maxHP)
        {
            this.owner = owner;
            this.maxHP = maxHP;
            transform.position = Camera.main.WorldToScreenPoint(owner.transform.position) + (Vector3)offset;
        }

        /// <summary>
        /// Updates the displayed health value of this health bar.
        /// </summary>
        /// <param name="health">The current health value of the attackable this object represents.</param>
        public void UpdateHealth(int health)
        {
            float normalizedHealth = (float)health / maxHP;
            fillImage.fillAmount = normalizedHealth;
        }

        /// <summary>
        /// Destroys this health bar.
        /// </summary>
        public void DestroyHealthBar()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Continually updates the health bar's position to align with it's owner's position.
        /// </summary>
        private void Update()
        {
            transform.position = Camera.main.WorldToScreenPoint(owner.transform.position) + (Vector3)offset;
        }
    }
}