/*****************************************************************************
// File Name : ISelectable.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Interface for player selectable objects with their mouse at runtime.
*****************************************************************************/
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Grubitecht
{
    public interface ISelectable
    {
        public Vector3 Position { get; }


        /// <summary>
        /// Handles specific behavoir that should happen upon selecting and deselecting this object.
        /// </summary>
        void OnSelect(ISelectable oldObj);

        void OnDeselect(ISelectable newObj);
    }

}