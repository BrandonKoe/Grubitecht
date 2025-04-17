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
        //[SerializeField, Tooltip("The height of this object.")]
        //private float height = 1f;
        //[SerializeField, Tooltip("Where in the world this object occupies space.  No two objects can occupy the same" +
        //    " space if they are grounded, and two flyers cannot share the same space.")]
        //private bool occupySpace = true;
        [field: SerializeField, Tooltip("The layer that this oblect occupies.  No two objects on the same layer can exist " +
            "on the same space")]
        public OccupyLayer Layer { get; private set; }
        //[SerializeField, Tooltip("Whether this object should adjust their height to be above any other objects that" +
        //    " are in the same space.  Only is relevant if OccupySpace is set to false.")]
        //private bool adjustHeight = false;
        //[field: SerializeField, Tooltip("Whether this object should be avoided by map navigators when run into.  " +
        //    "Should only be true of other enemies.")]
        //public bool CauseAvoidance { get; private set; } = true;
        // Note: This is the position of the voxel we are standing on.
        public VoxelTile CurrentTile { get; set; }

        //private readonly static List<GridObject> allObjectList = new List<GridObject>();

        public event Action OnChangeSpace;

        /// <summary>
        /// Assigns this object a space when it is created.
        /// </summary>
        private void Awake()
        {
            //base.Awake();
            //allObjectList.Add(this);
            SetCurrentSpace(VoxelTilemap3D.Main_GetApproximateSpace(transform.position));
            SnapToSpace();
            //Debug.Log(CurrentSpace.ToString());
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
            // Only change spaces if we're able to occupy the new space
            if (!newTile.ContainsObjectOnLayer(Layer))
            {
                if (oldSpace != null)
                {
                    oldSpace.RemoveObject(this);
                }
                newTile.AddObject(this);
                CurrentTile = newTile;
                //Debug.Log(name + " changed space");
                OnChangeSpace?.Invoke();
            }
            else
            {
                return;
            }
            //// Only assign the space's contained object value if this object is set to occupy space.
            //if (occupySpace)
            //{
            //    GridObject objInSpace = newTile.ContainedObject;
            //    //Debug.Log(objInSpace);
            //    // Two objects that occupy space cannot exist on the same space at once.
            //    if (objInSpace != null && objInSpace != this)
            //    {
            //        return;
            //    }
            //    // Update the spaces' references to their contained object.  We have left our previous space and are
            //    // now at the next space.
            //    if (CurrentTile != null)
            //    {
            //        CurrentTile.ContainedObject = null;
            //    }
            //    newTile.ContainedObject = this;
            //    // Invokes the OnMapRefresh event so that paths can be updated based on changes to the map.
            //    // Only need to refresh the map if it has changed due to the movement of an object that occupies space.
            //    // Switching this system to one where grid navigators only re-evaluate paths if they run into
            //    // a problem, not each time the map changes.
            //    //RefreshMap(this, oldSpace, CurrentSpace);
            //}
            //CurrentTile = newTile;
            ////Debug.Log(name + " changed space");
            //OnChangeSpace?.Invoke();
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
            // Ensures that if the object we're passing through is higher than our base offset, then we move high
            // enough to go over it.
            Vector3 oSet = offset;
            //if (adjustHeight && !occupySpace && tile.ContainsObject && tile.ContainedObject != this)
            //{
            //    oSet.y = Mathf.Max(oSet.y, tile.ContainedObject.height);
            //}
            //Debug.Log(space);
            return VoxelTilemap3D.Main_GridToWorldPos(tile.GridPosition) + oSet;
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