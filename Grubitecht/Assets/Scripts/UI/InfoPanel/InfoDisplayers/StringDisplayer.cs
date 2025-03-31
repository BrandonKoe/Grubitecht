/*****************************************************************************
// File Name : StringDisplayer.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Controls the displaying of strings on the info panel.
*****************************************************************************/
using TMPro;
using UnityEngine;

namespace Grubitecht.UI.InfoPanel
{
    [RequireComponent(typeof(TMP_Text))]
    public class StringDisplayer : InfoDisplayer<StringValue>
    {
        [SerializeField] private TMP_Text text;

        /// <summary>
        /// initializes this string to display the proper text and font size.
        /// </summary>
        /// <param name="valueRef">The stringValue that this displayer is displaying.</param>
        public override void Initialize(StringValue valueRef)
        {
            base.Initialize(valueRef);
            text.text = valueRef.Value;
            text.fontSize = valueRef.FontSize;
        }
    }
}