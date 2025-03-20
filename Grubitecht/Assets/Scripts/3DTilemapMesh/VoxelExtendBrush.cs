/*****************************************************************************
// File Name : VoxelExtendBrush.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Brush for painting positions on the 3D mesh tilemap that will extend wall tiles down to lower
// layers.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    [CreateAssetMenu]
    [CustomGridBrush(false, true, false, "Voxel Extend Brush")]
    public class VoxelExtendBrush : VoxelBrush
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
