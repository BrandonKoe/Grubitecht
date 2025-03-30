/*****************************************************************************
// File Name : DamageNumber.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Controls prefabs that are used to indicate the damage dealt to combatants.
*****************************************************************************/
using System.Collections;
using TMPro;
using UnityEngine;

namespace Grubitecht.UI
{
    [RequireComponent(typeof(Animator))]
    public class DamageNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        #region Component References
        [SerializeReference, HideInInspector] private Animator animator;
        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            animator = GetComponent<Animator>();
        }
        #endregion

        /// <summary>
        /// Initializes this damage number with the correct dispnaw number and coroutine for death.
        /// </summary>
        /// <param name="healthChange"></param>
        public void Initialize(int healthChange, Color color)
        {
            text.color = color;
            text.text = healthChange.ToString();
            // Destroy this text once it's animation has finished.
            StartCoroutine(DeathRoutine());
        }

        /// <summary>
        /// Destroys this object after it's animation is finished.
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator DeathRoutine()
        {
            yield return new WaitForSeconds(animator.GetAnimationDuration() - 0.02f);
            Destroy(gameObject);
        }
    }
}
