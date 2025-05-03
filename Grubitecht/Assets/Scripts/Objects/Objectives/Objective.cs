/*****************************************************************************
// File Name : Objective.cs
// Author : Brandon Koederitz
// Creation Date : March 19, 2025
//
// Brief Description : Controls objectives that enemies must destroy to defeat the player.
*****************************************************************************/
using Grubitecht.Combat;
using Grubitecht.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.World.Objects
{
    [RequireComponent(typeof(GridObject))]
    [RequireComponent(typeof(Attackable))]
    public class Objective : MonoBehaviour
    {
        [SerializeField, Tooltip("The order in which enemies will go after objectives.  Lower numbers will be " +
            "attacked first.")] 
        public int targetingOrder;
        [SerializeField] private Modifier<Attacker> onDeathModifier;
        //public static readonly BufferedNavigationMap NavMap = new BufferedNavigationMap(GetObjectivePositions, 1, 5f);
        private static List<Objective> currentObjectives = new();

        #region Component References
        [field: SerializeReference, HideInInspector] public GridObject gridObject {  get; private set; }
        [SerializeReference, HideInInspector] private Attackable attackable;

        public Attackable Attackable
        {
            get
            {
                return attackable;
            }
        }


        /// <summary>
        /// Assign compomnent references on reset.
        /// </summary>
        private void Reset()
        {
            gridObject = GetComponent<GridObject>();
            attackable = GetComponent<Attackable>();
        }
        #endregion

        #region Properties
        public static Objective TargetObjective
        {
            get
            {
                if (currentObjectives.Count == 0)
                {
                    return null;
                }
                return currentObjectives[0];
            }
        }
        public static List<Objective> CurrentObjectives
        {
            get
            {
                return currentObjectives;
            }
        }
        #endregion

        /// <summary>
        /// Add this objective to the list of current objectives when it awakes & subscribe to the Attacker OnDeath
        /// event.
        /// </summary>
        private void Awake()
        {
            currentObjectives.Add(this);
            currentObjectives = currentObjectives.OrderBy(item => item.targetingOrder).ToList();
            attackable.OnDeath += OnDeath;
            // Need to update the nav map whenever the objective changes spaces.  This may cause lag.
            //gridObject.OnChangeSpace += UpdateNavMap;
        }

        private void OnDestroy()
        {
            attackable.OnDeath -= OnDeath;
            currentObjectives.Remove(this);
            //gridObject.OnChangeSpace -= UpdateNavMap;
        }

        /// <summary>
        /// Gets the current spaces of all objectives.
        /// </summary>
        /// <returns>An array of spaces representing the spaces of all objectives.</returns>
        private static VoxelTile[] GetObjectivePositions()
        {
            return currentObjectives.Select(item => item.gridObject.CurrentTile).ToArray();
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
        /// Gets the objective thats the furthest away from a given position.
        /// </summary>
        /// <param name="position">The position to get the furthest objective from.</param>
        /// <returns>The objective that is furthest from that position.</returns>
        public static Objective GetFurthestObjective(Vector3 position)
        {
            // Prevents Index out of range.
            if (currentObjectives.Count == 0) { return null; }
            // Loop through all current objectives and compare the distance between them and the given position
            // to the currently stored lowest distance.
            Objective highestDistObj = currentObjectives[0];
            float highestDist = Vector3.Distance(position, highestDistObj.transform.position);
            foreach (Objective obj in currentObjectives)
            {
                float dist = Vector3.Distance(position, obj.transform.position);
                if (dist > highestDist)
                {
                    highestDist = dist;
                    highestDistObj = obj;
                }
            }
            return highestDistObj;
        }

        /// <summary>
        /// Handles behaviour that should happen when this objective dies.
        /// </summary>
        protected virtual void OnDeath()
        {
            currentObjectives.Remove(this);
            // Grants all enemies on the field a buff when an objective is destroyed.
            foreach(EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                if (enemy.TryGetComponent(out Attacker atk))
                {
                    atk.ApplyModifier(onDeathModifier);
                }
            }
            GrubManager.UpdateText();
            if (currentObjectives.Count == 0)
            {
                LevelManager.LoseLevel();
            }
        }

    }
}