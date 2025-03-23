/*****************************************************************************
// File Name : RuleModel.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Controls the composite model of this object that adapts to the placement of other similar
// rule models around it.
*****************************************************************************/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.OldTilemaps
{
    public class RuleModel : MonoBehaviour
    {
        [SerializeField] private Transform modelContainer;
        [field: SerializeField] public RuleTile3D RuleTile { get; private set; }

        // model settings
        [SerializeField] private readonly Dictionary<RuleTile3D.ModelInfo, GameObject> activeModelDict = new();
        [SerializeField, ReadOnly] private AdjacentTileInfo adjacentTileInfo;

        //// Non-Composite model settings
        //private RuleTile3D.ModelInfo dominantModel;
        //private GameObject modelObject;

        /// <summary>
        /// Updates this rule model on tile creation based on adjacent tile info passed in by the brush.
        /// </summary>
        /// <param name="adjInfo">Info about the tiles adjacent to this one.</param>
        public void SetRuleModel(AdjacentTileInfo adjInfo)
        {
            //Debug.Log("Rule Model Set");
            adjacentTileInfo = adjInfo;
            RuleTile.BakeModel(adjacentTileInfo, modelContainer, activeModelDict);
        }

        /// <summary>
        /// Updates the adjacent tiles information this tile stores with new information.
        /// </summary>
        /// <param name="tile">The information on the tile that is now adjacent to this one.</param>
        /// <param name="direction">The direction from this tile to the passed in tile.</param>
        public void UpdateFace(Tile3D tile, Vector3Int direction)
        {
            if (adjacentTileInfo.ContainsKey(direction))
            {
                adjacentTileInfo.Set(direction, tile);
            }
            else
            {
                adjacentTileInfo.Add(direction, tile);
            }
            RuleTile.BakeModel(adjacentTileInfo, modelContainer, activeModelDict);
        }
    } 
}