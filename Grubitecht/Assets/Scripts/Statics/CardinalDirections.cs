/*****************************************************************************
// File Name : CardinalDirections.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Class that allows for easy access to the cardinal directions as a collection.
*****************************************************************************/
using UnityEngine;

public static class CardinalDirections
{
    public static readonly Vector3Int[] ORTHOGONAL_3D = new Vector3Int[]
    {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.forward,
            Vector3Int.back
    };

    public static readonly Vector2Int[] ORTHOGONAL_2D = new Vector2Int[]
    {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
    };

    public static readonly Vector2Int[] DIAGONAL_2D = new Vector2Int[]
    {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down,
        new(1,1),
        new(1,-1),
        new(-1,1),
        new(-1,-1)
    };
}
