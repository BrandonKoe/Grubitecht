/*****************************************************************************
// File Name : Tile3D.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Foundational component for game objects that act as 3D tiles.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    public class Tile3D : MonoBehaviour
    {
        [field: SerializeField] public RuleModel RuleModel { get; private set; }
    }

}