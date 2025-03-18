/*****************************************************************************
// File Name : GridObject.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows an object to take up space in the world grid.
*****************************************************************************/
using Grubitecht.Tilemaps;
using UnityEngine;

namespace Grubitecht.World
{
    public class GridObject : MonoBehaviour
    {
        [SerializeField, Tooltip("The offset from the tile's position that this object should be at while on that " +
            "tile.")] 
        private Vector3 offset;
        public GroundTile CurrentSpace { get; set; }

        /// <summary>
        /// Assigns this object a space when it is created.
        /// </summary>
        private void Start()
        {
            Vector2Int approxGridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x - 
                Tile3DBrush.CELL_SIZE / 2), Mathf.RoundToInt(transform.position.z - Tile3DBrush.CELL_SIZE / 2));
            SetCurrentSpace(GroundTile.GetTileAt(approxGridPos));
            //Debug.Log(CurrentSpace.ToString());
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

        /// <summary>
        /// Gets the world space position this object should occupy when it is on a given tile.
        /// </summary>
        /// <param name="tile">The tile to get the position for this object of.</param>
        /// <returns>The position of the tile plus the set offset of this object.</returns>
        public Vector3 GetTilePosition(GroundTile tile)
        {
            return tile.transform.position + offset;
        }
    }
}