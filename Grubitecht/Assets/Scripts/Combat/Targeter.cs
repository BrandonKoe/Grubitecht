/*****************************************************************************
// File Name : Targeter.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Handles selecting nearby targets for an attacker.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(SphereCollider))]
    public class Targeter : MonoBehaviour
    {
        [SerializeField] private float detectionRange;
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


    }
}