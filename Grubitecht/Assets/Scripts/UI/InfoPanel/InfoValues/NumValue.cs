/*****************************************************************************
// File Name : IntValue.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Contains information about displaying an int on the info panel.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.UI.InfoPanel
{
    public class NumValue : InfoValue<float>
    {
        public float FontSize { get; set; } = DEFAULT_FONT_SIZE;
        public string NumName { get; set; }
        public string NumSuffix { get; set; }
        public Sprite Icon { get; set; }

        public NumValue(float value, int priority) : base(value, priority) { }

        public NumValue(float value, int priority, float fontSize) : base(value, priority)
        {
            FontSize = fontSize;
        }

        public NumValue(float value, int priority, string name) : base(value, priority)
        {
            NumName = name;
        }

        public NumValue(float value, int priority, string name, string suffix) : base(value, priority)
        {
            NumName = name;
            NumSuffix = suffix;
        }

        public NumValue(float value, int priority, Sprite icon) : base(value, priority)
        {
            Icon = icon;
        }

        public NumValue(float value, int priority, string name, string suffix, Sprite icon) : base(value, priority)
        {
            NumName = name;
            NumSuffix = suffix;
            Icon = icon;
        }
    }
}