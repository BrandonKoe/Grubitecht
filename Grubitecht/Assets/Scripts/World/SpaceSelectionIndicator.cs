/*****************************************************************************
// File Name : SpaceSelectionIndicator.cs
// Author : Brandon Koederitz
// Creation Date : March 21, 2025
//
// Brief Description : Indicates what space the player has selected.
*****************************************************************************/
using System;
using UnityEngine;

namespace Grubitecht.World
{
    public class SpaceSelectionIndicator : SelectionIndicator
    {
        [SerializeField] private Vector3 offset;
        public override Type[] SelectedComponentTypes => new Type[]
        {
            typeof(SpaceSelection)
        };

        public override void IndicateSelected(ISelectable selection)
        {
            if (selection is SpaceSelection space)
            {
                transform.position = space.WorldPosition + offset;
            }
        }
    }
}