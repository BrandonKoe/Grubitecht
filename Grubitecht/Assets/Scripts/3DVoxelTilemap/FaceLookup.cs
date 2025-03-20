/*****************************************************************************
// File Name : FaceLookup.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Separate class to handle getting the relevant vertex offsets of a face of a voxel mesh facing a
// given direction.  Doing it manually to ensure that offsets are given in the correct order when assigning triangles
// clockwise.
*****************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    public static class FaceLookup
    {
        private static readonly Dictionary<Vector3Int, Vector3Int[]> vertexLookup = new Dictionary<Vector3Int, Vector3Int[]>
        {
            {Vector3Int.right, new Vector3Int[4]
                {
                    Vector3Int.right,
                    Vector3Int.right + Vector3Int.up,
                    Vector3Int.right + Vector3Int.up + Vector3Int.forward,
                    Vector3Int.right + Vector3Int.forward,
                }
            },
            {Vector3Int.left, new Vector3Int[4]
                {
                    Vector3Int.forward,
                    Vector3Int.forward + Vector3Int.up,
                    Vector3Int.up,
                    Vector3Int.zero
                }
            },
            {Vector3Int.up, new Vector3Int[4]
                {
                    Vector3Int.up,
                    Vector3Int.up + Vector3Int.forward,
                    Vector3Int.up + Vector3Int.forward + Vector3Int.right,
                    Vector3Int.up + Vector3Int.right
                }
            },
            {Vector3Int.down, new Vector3Int[4]
                {
                    Vector3Int.forward,
                    Vector3Int.zero,
                    Vector3Int.right,
                    Vector3Int.right + Vector3Int.forward
                }
            },
            {Vector3Int.forward, new Vector3Int[4]
                {
                    Vector3Int.right + Vector3Int.forward,
                    Vector3Int.right + Vector3Int.forward + Vector3Int.up,
                    Vector3Int.forward + Vector3Int.up,
                    Vector3Int.forward
                }
            },
            {Vector3Int.back, new Vector3Int[4]
                {
                    Vector3Int.zero,
                    Vector3Int.up,
                    Vector3Int.right + Vector3Int.up,
                    Vector3Int.right
                }
            },

        };

        public static Vector3Int[] GetVertexOffsets(Vector3Int direction)
        {
            return vertexLookup[direction];
        }
    }
}