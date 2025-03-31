/*****************************************************************************
// File Name : IntValue.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Contains information about displaying an independent sprite on the info panel.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.UI.InfoPanel
{
    public class SpriteValue : InfoValue<Sprite>
    {
        public SpriteValue(Sprite value, int priority) : base(value, priority) { }
    }
}