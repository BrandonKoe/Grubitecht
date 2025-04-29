using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grubitecht.UI
{
    public class TweenedObject : UIObject
    {
        [SerializeField] private Transform aniamtedObject;
        [SerializeField] private Image[] fadeImages;
        [SerializeField] private TMP_Text[] fadeText;
        [Header("Settings")]
        [SerializeField] private float animationTime;
        [SerializeField] private Vector3 tweenOffset;
        [SerializeField] private float startingAlpha;
        [SerializeField] private float targetAlpha;

        /// <summary>
        /// Animates the object based on given parameters.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator LifeCycle()
        {
            Vector3 startingPos = aniamtedObject.localPosition;
            Vector3 endingPos = aniamtedObject.localPosition + tweenOffset;
            float timer = animationTime;
            while (timer > 0)
            {
                float normalizedTime = timer / animationTime;

                aniamtedObject.localPosition = Vector3.Lerp(startingPos, endingPos, normalizedTime);

                // Animates the alpha values of the object.
                float currentAlpha = Mathf.Lerp(startingAlpha, targetAlpha, normalizedTime);
                foreach(var image in fadeImages)
                {
                    image.color.SetAlpha(currentAlpha);
                }
                foreach(var text in fadeText)
                {
                    text.color.SetAlpha(currentAlpha);
                }

                timer -= Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}