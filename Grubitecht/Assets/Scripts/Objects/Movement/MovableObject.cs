/*****************************************************************************
// File Name : MovableObject.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows an object to be selected and moved along the world grid by the player.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.Waves;
using Grubitecht.World.Pathfinding;
using System.Collections;
using UnityEngine;

namespace Grubitecht.World.Objects
{
    [RequireComponent(typeof(PathNavigator))]
    [RequireComponent(typeof(SelectableObject))]
    public class MovableObject : MonoBehaviour
    {
        [SerializeField] private GridObject placeholderGridObjectPrefab;
        #region Component References
        [field: SerializeReference, HideInInspector] public PathNavigator GridNavigator { get; private set; }
        [SerializeReference, HideInInspector] private SelectableObject selectable;
        /// <summary>
        /// Assign component references.
        /// </summary>
        private void Reset()
        {
            GridNavigator = GetComponent<PathNavigator>();
            selectable = GetComponent<SelectableObject>();
        }
        #endregion

        /// <summary>
        /// Subscribe/unsubscribe from the OnDeselectEvent to control movement.
        /// </summary>
        private void Awake()
        {
            selectable.OnDeselectEvent += MoveObject;
        }
        private void OnDestroy()
        {
            selectable.OnDeselectEvent -= MoveObject;
            // Ensures any grubs are returned if this object is destroyed.
            GrubManager.ReturnGrub(this);
        }

        /// <summary>
        /// Navigates this object to a new selected space when it is deselected.
        /// </summary>
        /// <param name="newObj">The newly selected object.</param>
        public void MoveObject(ISelectable newObj)
        {
            // If the player selects a ground tile right after selecting a moveable object, then the object should
            // move to that selected position.
            if (newObj is SpaceSelection space)
            {
                if (GridNavigator.IsMoving || GrubManager.CheckGrub())
                {
                    // Check for invalid paths here.
                    if (CheckValidSpace(space.Tile))
                    {
                        GridNavigator.SetDestination(space.Tile, HandleMovementCallback);
                        GrubManager.AssignGrub(this);
                    }
                    else
                    {
                        // Invalid space.
                        Debug.Log("Invalid Space");
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the space this object is going to occupy results in a null path from the spawn point to the
        /// objective.
        /// </summary>
        /// <returns></returns>
        private bool CheckValidSpace(VoxelTile targetTile)
        {
            // Spawns a dummy grid object to take up space during the pathfind.
            GameObject go = new GameObject();
            GridObject tempGridObj = go.AddComponent<GridObject>();
            tempGridObj.Layer = GridNavigator.gridObject.Layer;
            tempGridObj.SetCurrentSpace(targetTile);
            tempGridObj.SnapToSpace();

            SpawnPoint[] spawnPoints = WaveManager.SpawnPoints;
            VoxelTile objectiveTile = Objective.TargetObjective.gridObject.CurrentTile;
            foreach(var spawnPoint in spawnPoints)
            {
                Debug.Log(spawnPoint.gridObject);
                // If a spawn point doesnt have a valid path to the target objective, then this space is not valid.
                if (!Pathfinder.CheckPath(spawnPoint.gridObject.CurrentTile, objectiveTile, 1, 
                    GridNavigator.gridObject.Layer, true))
                {
                    // Call OnDestroy here manuallt
                    tempGridObj.DestroyImmediate();
                    return false;
                }
            }
            tempGridObj.DestroyImmediate();
            return true;
        }

        /// <summary>
        /// Recalls a grub delegated to moving this object from the grub controller.
        /// </summary>
        private void HandleMovementCallback(PathStatus pathStatus)
        {
            StartCoroutine(DelayedMovementCallback(pathStatus));   
        }

        /// <summary>
        /// Need to delay a frame before we handle the movement callback to ensure that grubs have been assigned
        /// before we handle returning them.
        /// </summary>
        /// <param name="pathStatus"></param>
        /// <returns></returns>
        private IEnumerator DelayedMovementCallback(PathStatus pathStatus)
        {
            yield return null;

            switch (pathStatus)
            {
                case PathStatus.Started:
                    break;
                case PathStatus.Completed:
                case PathStatus.Invalid:
                    // Return the grub if the path status is completed or the path is invalid.
                    GrubManager.ReturnGrub(this);
                    break;
                default:
                    break;
            }
        }
    }
}