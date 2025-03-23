/*****************************************************************************
// File Name : MathHelpers.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Set of functions for common mathematical calculations.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht
{
    public static class MathHelpers
    {
        /// <summary>
        /// Clamps all values of a vector3int between two given ints.
        /// </summary>
        /// <param name="input">The vector to clamp.</param>
        /// <param name="min">The min possible value.</param>
        /// <param name="max">The max possible value.</param>
        /// <returns>The Vector3Int with all of it's components clamped between the min and max values.</returns>
        public static Vector3Int V3IntClamp(Vector3Int input, int min, int max)
        {
            input.x = Mathf.Clamp(input.x, min, max);
            input.y = Mathf.Clamp(input.y, min, max);
            input.z = Mathf.Clamp(input.z, min, max);
            return input;
        }

        /// <summary>
        /// Gets the absolute value of a vector's components.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <returns>The passed in vector with all of its components as positve values.</returns>
        public static Vector3Int V3IntAbs(Vector3Int input)
        {
            input.x = Mathf.Abs(input.x);
            input.y = Mathf.Abs(input.y);
            input.z = Mathf.Abs(input.z);
            return input;   
        }
    }
}