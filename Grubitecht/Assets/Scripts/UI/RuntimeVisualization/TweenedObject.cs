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
        [SerializeField, Tooltip("The offset from the object's current position that it should tween to.")]
        private Vector3 tweenOffset;
        [SerializeField] private AnimationCurve alphaCurve;

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
                float normalizedProgress = 1 - (timer / animationTime);

                aniamtedObject.localPosition = Vector3.Lerp(startingPos, endingPos, normalizedProgress);

                // Animates the alpha values of the object.
                //float currentAlpha = Mathf.Lerp(startingAlpha, endingAlpha, normalizedProgress);
                float currentAlpha = alphaCurve.Evaluate(normalizedProgress);
                Color col;
                foreach(var image in fadeImages)
                {
                    col = image.color;
                    col.a = currentAlpha;
                    image.color = col;
                }
                foreach(var text in fadeText)
                {
                    col = text.color;
                    col.a = currentAlpha;
                    text.color = col;
                }

                timer -= Time.unscaledDeltaTime;
                yield return null;
            }
            EndLifeCycle();
            //Destroy(gameObject);
        }
    }
}