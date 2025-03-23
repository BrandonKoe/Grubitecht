/*****************************************************************************
// File Name : Attackable.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Allows an object to have health and be attacked by attackers.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class Attackable : MonoBehaviour
    {
        [field: SerializeField] public Team Team { get; private set; }

        /// <summary>
        /// Update rigidbody values on reset, as the rigidbody should always be kinematic and never use gravity.
        /// </summary>
        private void Reset()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

}