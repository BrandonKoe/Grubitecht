/*****************************************************************************
// File Name : GridBehaviour.cs
// Author : Brandon Koederitz
// Creation Date : March 19, 2025
//
// Brief Description : Base class for objects that exist on the world grid.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Grubitecht.World.Objects;

namespace Grubitecht.World
{
    [Obsolete("Grid Behaviour is obsolete.  Check for roadblocks as a path is executed through instead and " +
        "re-evaluate when a problem is found.")]
    public abstract class GridBehaviour : MonoBehaviour
    {
        // Event to call functions that should run whenever a grid object is moved to a new space.
        public static event Action<GridObject, Vector3Int, Vector3Int> OnMapRefreshEvent;

        /// <summary>
        /// Subscribe and unsubscribe OnMapRefresh to the refresh event
        /// </summary>
        protected virtual void Awake()
        {
            OnMapRefreshEvent += OnMapRefresh;
        }
        protected virtual void OnDestroy()
        {
            OnMapRefreshEvent -= OnMapRefresh;
        }

        /// <summary>
        /// Overwrittable function that implements behaviour that should run whenever the map refreshes such as
        /// updating pathfinding.
        /// </summary>
        /// <param name="movedObject">The object that was moved during this map refresh.</param>
        /// <param name="oldSpace">The old tile position of the moved object.</param>
        /// <param name="newSpace">The new tile position of the moved object.</param>
        protected virtual void OnMapRefresh(GridObject movedObject, Vector3Int oldSpace, Vector3Int newSpace) { }
        
        /// <summary>
        /// Calls OnMapRefresh for all GridBehaviours
        /// </summary>
        /// <param name="movedObject">The object that was moved during this map refresh.</param>
        /// <param name="oldSpace">The old tile position of the moved object.</param>
        /// <param name="newSpace">The new tile position of the moved object.</param>
        protected void RefreshMap(GridObject movedObject, Vector3Int oldSpace, Vector3Int newSpace)
        {
            OnMapRefreshEvent?.Invoke(movedObject, oldSpace, newSpace);
        }
    }
}