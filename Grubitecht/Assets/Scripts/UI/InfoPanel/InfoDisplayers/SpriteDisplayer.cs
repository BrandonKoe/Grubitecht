/*****************************************************************************
// File Name : StringDisplayer.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Controls the displaying of strings on the info panel.
*****************************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace Grubitecht.UI.InfoPanel
{
    [RequireComponent(typeof(Image))]
    public class SpriteDisplayer : InfoDisplayer<SpriteValue>
    {
        #region Component References
        [SerializeReference, HideInInspector] private Image image;
        /// <summary>
        /// Assign component references on reset.
        /// </summary>
        private void Reset()
        {
            image = GetComponent<Image>();
        }
        #endregion

        /// <summary>
        /// initializes this int to display the proper name, icon, and font size.
        /// </summary>
        /// <param name="valueRef">The IntValue that this displayer is displaying.</param>
        public override void Initialize(SpriteValue valueRef)
        {
            base.Initialize(valueRef);
            image.sprite = valueRef.Value;
        }
    }
}