/*****************************************************************************
// File Name : GridObject.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows an object to take up space in the world grid.
*****************************************************************************/
using Grubitecht.Tilemaps;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Grubitecht.World.Objects
{
    public class GridObject : MonoBehaviour
    {
        #region CONSTS
        //public const TileType VALID_GROUND_TYPE = TileType.Ground;
        #endregion

        [SerializeField, Tooltip("The offset from the tile's position that this object should be at while on that " +
            "tile.")] 
        private Vector3 offset = new Vector3(0, 0.5f, 0);
        [SerializeField, Tooltip("Whether this object should occupy space in the world.  If true then other objects" +
            " that occupy space cannot be inside the same space as this object.")]
        private bool occupySpace = true;
        //[field: SerializeField, Tooltip("Whether this object should be avoided by map navigators when run into.  " +
        //    "Should only be true of other enemies.")]
        //public bool CauseAvoidance { get; private set; } = true;
        // Note: This is the position of the voxel we are standing on.
        [field: SerializeField, ReadOnly] public VoxelTile CurrentTile { get; set; }

        //private readonly static List<GridObject> allObjectList = new List<GridObject>();

        public event Action OnChangeSpace;

        /// <summary>
        /// Assigns this object a space when it is created.
        /// </summary>
        private void Awake()
        {
            //base.Awake();
            //allObjectList.Add(this);
            SetCurrentSpace(GetApproximateSpace());
            SnapToSpace();
            //Debug.Log(CurrentSpace.ToString());
        }

        private void OnDestroy()
        {
            //base.OnDestroy();
            //allObjectList.Remove(this);
        }

        /// <summary>
        /// Gets an approximation of the space that this object's transform is currently at.
        /// </summary>
        /// <returns>The space that this object's transform is physically at in world space.</returns>
        public VoxelTile GetApproximateSpace()
        {
            Vector2Int approxSpace = new Vector2Int();
            approxSpace.x = Mathf.RoundToInt(transform.position.x - VoxelTilemap3D.CELL_SIZE / 2);
            approxSpace.y = Mathf.RoundToInt(transform.position.z - VoxelTilemap3D.CELL_SIZE / 2);
            //Debug.Log(approxSpace);
            // Gets a list of possible spaces this object could exist at based on it's 2D position and then finds
            // the one with the closest elevation.  This ensures that the object snaps from gravity.
            return VoxelTilemap3D.Main_GetTile(approxSpace);
        }

        /// <summary>
        /// Moves this object to a new space.
        /// </summary>
        /// <param name="newTile">The space to move this object to.</param>
        public void SetCurrentSpace(VoxelTile newTile)
        {
            //// Cant set our space to a space that doesnt exist.
            //if (!VoxelTilemap3D.Main_CheckCell(newSpace)) { Debug.Log("Invalid Space " + newSpace); return; }
            VoxelTile oldSpace = CurrentTile;
            // Only assign the space's contained object value if this object is set to occupy space.
            if (occupySpace)
            {
                GridObject objInSpace = newTile.ContainedObject;
                //Debug.Log(objInSpace);
                // Two objects that occupy space cannot exist on the same space at once.
                if (objInSpace != null && objInSpace != this)
                {
                    return;
                }
                // Update the spaces' references to their contained object.  We have left our previous space and are
                // now at the next space.
                CurrentTile.ContainedObject = null;
                newTile.ContainedObject = this;
                // Invokes the OnMapRefresh event so that paths can be updated based on changes to the map.
                // Only need to refresh the map if it has changed due to the movement of an object that occupies space.
                // Switching this system to one where grid navigators only re-evaluate paths if they run into
                // a problem, not each time the map changes.
                //RefreshMap(this, oldSpace, CurrentSpace);
            }
            CurrentTile = newTile;
            //Debug.Log(name + " changed space");
            OnChangeSpace?.Invoke();
        }

        /// <summary>
        /// Snaps this object to the position it should occupy for it's current space.
        /// </summary>
        public void SnapToSpace()
        {
            // Cant snap to a space if we dont have one.
            if (CurrentTile == null) { return; }
            transform.position = GetOccupyPosition(CurrentTile);
        }

        /// <summary>
        /// Gets the world space position this object should occupy when it is on a given tile.
        /// </summary>
        /// <param name="tile">The tile to get the position for this object of.</param>
        /// <returns>The position of the tile plus the set offset of this object.</returns>
        public Vector3 GetOccupyPosition(VoxelTile tile)
        {
            //Debug.Log(space);
            return VoxelTilemap3D.Main_GridToWorldPos(tile.GridPosition) + offset;
        }

        ///// <summary>
        ///// Gets the object at a given space.
        ///// </summary>
        ///// <param name="space">The space to get the object from.</param>
        ///// <returns>The object at that space.</returns>
        //public static GridObject GetObjectAtSpace(Vector3Int space)
        //{
        //    return allObjectList.Find(item => item.CurrentSpace == space && item.occupySpace);
        //}

        ///// <summary>
        ///// Checks if a given space is occupied.
        ///// </summary>
        ///// <param name="space">The space to check.</param>
        ///// <returns>True if the space is occupied.</returns>
        //public static bool CheckOccupied(Vector3Int space)
        //{
        //    return allObjectList.Find(item => item.CurrentSpace == space && item.occupySpace) != null;
        //}
    }
}