/*****************************************************************************
// File Name : GridObject.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows an object to take up space in the world grid.
*****************************************************************************/
using Grubitecht.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht
{
    public class GridObject : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        public GroundTile CurrentSpace { get; set; }

        /// <summary>
        /// Assigns this object a space when it is created.
        /// </summary>
        private void Start()
        {
            Vector2Int approxGridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x - 
                Tile3DBrush.CELL_SIZE / 2), Mathf.RoundToInt(transform.position.z - Tile3DBrush.CELL_SIZE / 2));
            SetCurrentSpace(GroundTile.GetTileAt(approxGridPos));
            Debug.Log(CurrentSpace.ToString());
        }

        /// <summary>
        /// Moves this object to a new space.
        /// </summary>
        /// <param name="space">The space to move this object to.</param>
        public void SetCurrentSpace(GroundTile space)
        {
            if (CurrentSpace != null)
            {
                CurrentSpace.ContainedObject = null;
            }
            CurrentSpace = space;
            if (CurrentSpace != null)
            {
                CurrentSpace.ContainedObject = this;
            }
        }
    }
}