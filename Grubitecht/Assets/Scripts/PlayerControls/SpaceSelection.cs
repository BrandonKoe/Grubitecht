/*****************************************************************************
// File Name : SpaceSelection.cs
// Author : Brandon Koederitz
// Creation Date : March 21, 2025
//
// Brief Description : Represents a space in the world that the player has selected.  Dummy class created by the
// selector when it clicks on the tilemap.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht
{
    public class SpaceSelection : ISelectable
    {
        public Vector3Int Position { get; set; }

        public SpaceSelection(Vector3Int pos)
        {
            Position = pos;
        }

        public void OnSelect(ISelectable oldObj)
        {
            // Nothing happens.
        }

        public void OnDeselect(ISelectable newObj)
        {
            // Nothing happens.
        }
    }
}
