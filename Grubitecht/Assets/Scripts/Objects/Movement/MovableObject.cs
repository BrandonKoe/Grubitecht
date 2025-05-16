/*****************************************************************************
// File Name : MovableObject.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows an object to be selected and moved along the world grid by the player.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.UI;
using Grubitecht.Waves;
using Grubitecht.World.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.World.Objects
{
    [RequireComponent(typeof(PathNavigator))]
    [RequireComponent(typeof(SelectableObject))]
    public class MovableObject : MonoBehaviour
    {
        [Header("Player Feedback Indicators")]
        [SerializeField] private TweenedObject invalidSpacePrefab;
        [SerializeField] private TweenedObject noGrubsPrefab;
        [SerializeField] private TweenedObject invalidPathPrefab;

        public static event Action<MovableObject, VoxelTile> OnObjectMoveStatic;
        public event Action<MovableObject, VoxelTile> OnObjectMove;

        public bool IsMovable { get; set; }
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

        #region Properties
        private GridObject gridObject => GridNavigator.gridObject;
        #endregion

        /// <summary>
        /// Subscribe/unsubscribe from the OnDeselectEvent to control movement.
        /// </summary>
        private void Awake()
        {
            selectable.OnDeselectEvent += MoveObject;
            IsMovable = true;
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
                    if (CheckValidObjectivePath(space.Tile))
                    {
                        GridNavigator.SetDestination(space.Tile, HandleMovementCallback);
                        GrubManager.AssignGrub(this);
                    }
                    else
                    {
                        // Invalid space.
                        //Debug.Log("Invalid Space");
                        if (invalidSpacePrefab != null)
                        {
                            // Spawns a UI object to communicate that the space is invalid.
                            WorldSpaceCanvasManager.SpawnUIObject(invalidSpacePrefab,
                                VoxelTilemap3D.Main_GridToWorldPos(space.Tile.GridPosition));
                        }
                    }
                }
                else
                {
                    if (noGrubsPrefab != null)
                    {
                        WorldSpaceCanvasManager.SpawnUIObject(noGrubsPrefab,
                            VoxelTilemap3D.Main_GridToWorldPos(space.Tile.GridPosition));
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the space this object is going to occupy results in a null path from the spawn point to the
        /// objective.
        /// </summary>
        /// <returns></returns>
        private bool CheckValidObjectivePath(VoxelTile targetTile)
        {
            // Temporarily move all grounded enemies to a placeholder layer so they dont iterfere with our pathfinding
            // check.
            List<EnemyController> ignoredEnemies = 
                EnemyController.AllEnemies.FindAll(item => item.gridObject.Layer == OccupyLayer.Ground);

            void SetEnemyLayer(OccupyLayer layer)
            {
                foreach (var enemy in ignoredEnemies)
                {
                    enemy.gridObject.Layer = layer;
                }
            }

            // Move the enemies to a temp layer.
            SetEnemyLayer(OccupyLayer.PlaceholderEnemy);

            // Temporarily sets this object's current tile as the tile we want to move it to.
            VoxelTile currentTille = gridObject.CurrentTile;
            gridObject.SetCurrentSpace(targetTile);

            // Define a CleanUp function that we can call to revert any changes before we exit this function.
            void CleanUp()
            {
                gridObject.SetCurrentSpace(currentTille);
                SetEnemyLayer(OccupyLayer.Ground);
            }

            //// Spawns a dummy grid object to take up space during the pathfind.
            //GameObject go = new GameObject();
            //GridObject tempGridObj = go.AddComponent<GridObject>();
            //tempGridObj.Layer = GridNavigator.gridObject.Layer;
            //tempGridObj.SetCurrentSpace(targetTile);
            //tempGridObj.SnapToSpace();

            SpawnPoint[] spawnPoints = WaveManager.SpawnPoints;
            VoxelTile objectiveTile = Objective.TargetObjective.gridObject.CurrentTile;
            if (spawnPoints != null)
            {
                foreach (var spawnPoint in spawnPoints)
                {
                    //Debug.Log(spawnPoint.gridObject);
                    // If a spawn point doesnt have a valid path to the target objective, then this space is not valid.
                    if (!Pathfinder.CheckPath(spawnPoint.gridObject.CurrentTile, objectiveTile, 1,
                        GridNavigator.gridObject.Layer, true))
                    {
                        // Call OnDestroy here manually
                        //tempGridObj.DestroyImmediate();
                        CleanUp();
                        return false;
                    }
                }
            }
            //tempGridObj.DestroyImmediate();
            CleanUp();
            return true;
        }

        /// <summary>
        /// Recalls a grub delegated to moving this object from the grub controller.
        /// </summary>
        private void HandleMovementCallback(PathCallbackInfo pathStatus)
        {
            StartCoroutine(DelayedMovementCallback(pathStatus));   
        }

        /// <summary>
        /// Need to delay a frame before we handle the movement callback to ensure that grubs have been assigned
        /// before we handle returning them.
        /// </summary>
        /// <param name="callbackInfo">Infor about the path that gave this callback.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator DelayedMovementCallback(PathCallbackInfo callbackInfo)
        {
            yield return null;

            switch (callbackInfo.Status)
            {
                case PathStatus.Started:
                    // Once this object starts moving, we should broadcast out that this object has started moving.
                    OnObjectMoveStatic?.Invoke(this, callbackInfo.EndTile);
                    OnObjectMove?.Invoke(this, callbackInfo.EndTile);
                    break;
                case PathStatus.Invalid:
                    if (invalidPathPrefab != null)
                    {
                        WorldSpaceCanvasManager.SpawnUIObject(invalidPathPrefab,
                            VoxelTilemap3D.Main_GridToWorldPos(callbackInfo.EndTile.GridPosition));
                    }
                    // Return the grub if the path status is completed or the path is invalid.
                    if (!GridNavigator.IsMoving)
                    {
                        GrubManager.ReturnGrub(this);
                    }
                    break;
                case PathStatus.Completed:
                    // Return the grub if the path status is completed or the path is invalid.
                    GrubManager.ReturnGrub(this);
                    break;
                default:
                    break;
            }
        }
    }
}