/*****************************************************************************
// File Name : StringDisplayer.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Controls the displaying of strings on the info panel.
*****************************************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grubitecht.UI.InfoPanel
{
    public class NumDisplayer : InfoDisplayer<NumValue>
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text text;

        /// <summary>
        /// initializes this int to display the proper name, icon, and font size.
        /// </summary>
        /// <param name="valueRef">The IntValue that this displayer is displaying.</param>
        public override void Initialize(NumValue valueRef)
        {
            base.Initialize(valueRef);
            string valueText = $"{valueRef.NumName}: {valueRef.Value}";
            text.text = valueText;
            text.fontSize = valueRef.FontSize;
            if (iconImage != null && valueRef.Icon != null)
            {
                iconImage.gameObject.SetActive(true);
                iconImage.sprite = valueRef.Icon;
            }
        }
    }
}