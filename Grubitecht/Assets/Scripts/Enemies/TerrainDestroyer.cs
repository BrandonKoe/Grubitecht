/*****************************************************************************
// File Name : TerrainDestroyer.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Allows enemies to destroy destructible terrain that is nearby to them.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.World.Objects
{
    [RequireComponent(typeof(SphereCollider))]
    public class TerrainDestroyer : MonoBehaviour
    {
        #region CONSTS
        private const string DESTRUCTIBLE_TAG = "DestructibleTerrain";
        #endregion
        [SerializeField, Min(0.1f)] private float detectionRange;

        #region Component References
        [SerializeReference, HideInInspector] private SphereCollider detectionArea;

        /// <summary>
        /// Assign component references on component reset.
        /// </summary>
        private void Reset()
        {
            detectionArea = GetComponent<SphereCollider>();
            detectionArea.isTrigger = true;
        }
        #endregion

        /// <summary>
        /// Automatically updates the sphere collider's radius based on the detection range of this component.
        /// </summary>
        private void OnValidate()
        {
            detectionArea.radius = detectionRange;
        }

        /// <summary>
        /// Destroys all destructible terrain when they come in range of this enemy.
        /// </summary>
        /// <param name="other">The object that has entered this object's range.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(DESTRUCTIBLE_TAG))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
