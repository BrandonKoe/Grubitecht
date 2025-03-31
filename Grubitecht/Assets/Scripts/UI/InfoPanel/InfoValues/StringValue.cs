/*****************************************************************************
// File Name : StringValue.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Contains information about displaying a string on the info panel.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.UI.InfoPanel
{
    public class StringValue : InfoValue<string>
    {
        public float FontSize { get; set; } = DEFAULT_FONT_SIZE;

        public StringValue(string value, int priority) : base(value, priority) { }

        public StringValue(string value, int priority, float fontSize) : base(value, priority)
        {
            FontSize = fontSize;
        }
    }
}