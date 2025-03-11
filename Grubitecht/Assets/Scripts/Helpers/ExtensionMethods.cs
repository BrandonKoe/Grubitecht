/*****************************************************************************
// File Name : ExtensionMethods.cs
// Author : Brandon Koederitz
// Creation Date : March 10, 2025
//
// Brief Description : Set of reusable extension methods that help with modifying certain data types.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Clamps all values of a Vector3 to a specific range
        /// </summary>
        /// <param name="value">The vector to clamp</param>
        /// <param name="min">The min value that the vector's components can have.</param>
        /// <param name="max">The max value that the vector's components can have.</param>
        public static void ClampVector(this Vector3Int value, int min, int max)
        {
            value.x = Mathf.Clamp(value.x, min, max);
            value.y = Mathf.Clamp(value.y, min, max);
            value.z = Mathf.Clamp(value.z, min, max);
        }
    }

}