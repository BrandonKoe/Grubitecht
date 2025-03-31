/*****************************************************************************
// File Name : DamageIndicator.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Displays changes to health for attackable objects.
*****************************************************************************/
using Grubitecht.Combat;
using UnityEngine;

namespace Grubitecht.UI
{
    public class DamageIndicator : MonoBehaviour
    {
        [SerializeField] private DamageNumber damageNumberPrefab;
        [SerializeField] private Color loseHealthColor;
        [SerializeField] private Color gainHealthColor;
        [SerializeField] private bool useWorldSpaceCanvas;
        private static DamageIndicator instance;

        /// <summary>
        /// Assign the singleton instance that will be used as the parent of the damage indicator UI objects.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("Multiple damage indicators exist.");
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
        /// Displays a number on the UI to represent the change in health for this attackable.
        /// </summary>
        /// <param name="healthChange">The change in the health of the attackable.</param>
        /// <param name="target">The attackable whose health changed.</param>
        public static void DisplayHealthChange(int healthChange, Attackable target, Color damageIndicatorColor)
        {
            DamageNumber num = Instantiate(instance.damageNumberPrefab, instance.transform);
            if (instance.useWorldSpaceCanvas)
            {
                num.transform.position = target.transform.position;
                Vector3 pos = num.transform.localPosition;
                pos.z = 0;
                num.transform.localPosition = pos;
            }
            else
            {
                num.transform.position = Camera.main.WorldToScreenPoint(target.transform.position);
            }
            //Color numColor = healthChange > 0 ? instance.gainHealthColor : instance.loseHealthColor;
            num.Initialize(healthChange, damageIndicatorColor);
        }
    }
}
