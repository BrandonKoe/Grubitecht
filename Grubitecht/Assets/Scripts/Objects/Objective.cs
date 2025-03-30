/*****************************************************************************
// File Name : Objective.cs
// Author : Brandon Koederitz
// Creation Date : March 19, 2025
//
// Brief Description : Controls objectives that enemies must destroy to defeat the player.
*****************************************************************************/
using Grubitecht.Combat;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grubitecht.World.Pathfinding;

namespace Grubitecht.World.Objects
{
    [RequireComponent(typeof(GridObject))]
    [RequireComponent(typeof(Attackable))]
    public class Objective : MonoBehaviour
    {
        public static readonly NavigationMap NavMap = new NavigationMap();
        private static readonly List<Objective> currentObjectives = new();

        #region Component References
        [field: SerializeReference, HideInInspector] public GridObject gridObject {  get; private set; }
        [SerializeReference, HideInInspector] private Attackable attackable;


        /// <summary>
        /// Assign compomnent references on reset.
        /// </summary>
        private void Reset()
        {
            gridObject = GetComponent<GridObject>();
            attackable = GetComponent<Attackable>();
        }
        #endregion

        /// <summary>
        /// Add this objective to the list of current objectives when it awakes & subscribe to the Attacker OnDeath
        /// event.
        /// </summary>
        private void Awake()
        {
            currentObjectives.Add(this);
            attackable.OnDeath += OnDeath;
            // Need to update the nav map whenever the objective changes spaces.  This may cause lag.
            gridObject.OnChangeSpace += UpdateNavMap;
        }

        private void OnDestroy()
        {
            attackable.OnDeath -= OnDeath;
            currentObjectives.Remove(this);
            gridObject.OnChangeSpace -= UpdateNavMap;
        }

        /// <summary>
        /// Gets the current spaces of all objectives.
        /// </summary>
        /// <returns>An array of spaces representing the spaces of all objectives.</returns>
        private static Vector3Int[] GetObjectivePositions()
        {
            return currentObjectives.Select(item => item.gridObject.CurrentSpace).ToArray();
        }

        /// <summary>
        /// Gets the closest objective to a given position.
        /// </summary>
        /// <param name="position">The position to get the nearest objective to.</param>
        /// <returns>The objective closest to the given position.</returns>
        public static Objective GetNearestObjective(Vector3 position)
        {
            // Prevents Index out of range.
            if (currentObjectives.Count == 0) { return null; }
            // Loop through all current objectives and compare the distance between them and the given position
            // to the currently stored lowest distance.
            Objective lowestDistObj = currentObjectives[0];
            float lowestDist = Vector3.Distance(position, lowestDistObj.transform.position);
            foreach (Objective obj in currentObjectives)
            {
                float dist = Vector3.Distance(position, obj.transform.position);
                if (dist < lowestDist)
                {
                    lowestDist = dist;
                    lowestDistObj = obj;
                }
            }
            return lowestDistObj;
        }

        /// <summary>
        /// Handles behaviour that should happen when this objective dies.
        /// </summary>
        private void OnDeath()
        {
            currentObjectives.Remove(this);
            if (currentObjectives.Count == 0)
            {

            }
            else
            {
                // Need to update the nav map when an objective is destroyed so that enemies dont funnel towards
                // an emmpty space.
                UpdateNavMap();
            }
        }

        /// <summary>
        /// Updates the objective nav map.
        /// </summary>
        public static void UpdateNavMap()
        {
            NavMap.UpdateMap(GetObjectivePositions());
        }
    }
}