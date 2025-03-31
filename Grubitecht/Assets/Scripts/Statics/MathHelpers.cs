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

        /// <summary>
        /// Returns the canonical modulus of a number to the mod of another number
        /// </summary>
        /// <remarks>
        /// Differs from % in that % gives the remainder, which can be negative.  In this case, negative number loop 
        /// around.
        /// </remarks>
        /// <param name="x">The first neumber</param>
        /// <param name="m">The number to take the mod of.</param>
        /// <returns>The canonical modulus number of X mod M.</returns>
        public static int Mod(int x, int m)
        {
            return ((x % m) + m) % m;
        }

        /// <summary>
        /// Gets the sign of a given number
        /// </summary>
        /// <param name="x">The number to get the sign of.</param>
        /// <returns>-1, 0, or 1, depending ont the sign of the number.</returns>
        public static int GetSign(int x)
        {
            if (x == 0) { return 0; }
            return Mathf.Abs(x) / x;
        }

        /// <summary>
        /// Converts an angle in degrees to a unit vector pointing in that direction.
        /// </summary>
        /// <param name="angle">The angle to convert to a vector.</param>
        /// <returns>A Vector2 corresponding to that angle.</returns>
        public static Vector2 DegAngleToUnitVector(float angle)
        {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        }

        /// <summary>
        /// Converts a vector to an angle in degrees.
        /// </summary>
        /// <param name="vector">The vector to convert to an angle.</param>
        /// <returns>The angle that corresponds to that vector.</returns>
        public static float VectorToDegAngle(Vector2 vector)
        {
            return Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Finds the Manhatten distance (or the total number of spaces between the two tiles when restricted to
        /// orthogonal movement) between two tiles.
        /// </summary>
        /// <param name="tile1">The first tile.</param>
        /// <param name="tile2">The second tile.</param>
        /// <returns>The total number of spaces between the two tiles.</returns>
        public static int FindManhattenDistance(Vector2Int tile1, Vector2Int tile2)
        {
            return Mathf.Abs(tile1.x - tile2.x) + Mathf.Abs(tile1.y - tile2.y);
        }
    }
}