/*****************************************************************************
// File Name : ButtonSelectAnimator.cs
// Author : Brandon Koederitz
// Creation Date : May 3, 2025
//
// Brief Description : Animates the fill of an image based on the selected status of a button.
*****************************************************************************/
using Grubitecht.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Grubitecht.UI
{
    public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private Sound selectSound;
        [SerializeField] private Sound clickSound;
        [Header("Settings")]
        [SerializeField] private float targetSelectedValue;
        [SerializeField] private float targetDeselectedValue;
        [SerializeField] private float animationTime;
        [SerializeField] private AnimationCurve animationCurve;

        private Coroutine animRoutine;

        /// <summary>
        /// Animates the fill of an image based on when this object is highlighted by the user's mouse cursor.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.PlaySoundAtPosition(selectSound, transform.position);
            AnimateTo(targetSelectedValue);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            AnimateTo(targetDeselectedValue);   
        }

        /// <summary>
        /// Reset the image's fill when this object is enabled to ensure it always starts as unselected.
        /// </summary>
        private void OnEnable()
        {
            targetImage.fillAmount = targetDeselectedValue;
        }


        /// <summary>
        /// Animates the fill of the target image based on an animation curve.
        /// </summary>
        /// <param name="target">The target fill amount the image should animate to.</param>
        /// <returns>Coroutine.</returns>
        private void AnimateTo(float target)
        {
            if (animRoutine != null)
            {
                StopCoroutine(animRoutine);
                animRoutine = null;
            }
            animRoutine = StartCoroutine(AnimateRoutine(target));
        }
        private IEnumerator AnimateRoutine(float target)
        {
            float initial = targetImage.fillAmount;
            float timer = animationTime;
            while (timer > 0)
            {
                float normalizedProgress = 1 - (timer / animationTime);
                targetImage.fillAmount = Mathf.Lerp(initial, target, animationCurve.Evaluate(normalizedProgress));

                timer -= Time.unscaledDeltaTime;
                yield return null;
            }
            animRoutine = null;
        }

        /// <summary>
        /// Play a sound when this object is clicked.
        /// </summary>
        /// <param name="eventData">Unused.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // Click sounds should always DontDestroyOnLoad in case the scene changes.
            AudioManager.PlaySoundAtPosition(clickSound, transform.position, true);
        }
    }
}