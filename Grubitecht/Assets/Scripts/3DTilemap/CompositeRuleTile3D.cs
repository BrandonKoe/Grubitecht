/*****************************************************************************
// File Name : CompositeRuleTile3D.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Set of model information for creating 3D tilemaps.  This RuleMode creates a composite model
// made up of individual models that exist simultaneously.
*****************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.OldTilemaps
{
    [CreateAssetMenu(fileName = "CompositeRuleTile3D", menuName = "Tilemap3D/Composite Rule Tile 3D")]
    public class CompositeRuleTile3D : RuleTile3D
    {
        /// <summary>
        /// Recreates the model for a RuleModel object based on it's adjacent tile info.
        /// </summary>
        /// <param name="adjInfo">Info about the tiles adjacent to the tile to change the model of.</param>
        /// <param name="parentTransform">The transform that will contain the model.</param>
        /// <param name="activeModel">The models that the rule model component is actively using.</param>
        public override void BakeModel(AdjacentTileInfo adjInfo, Transform parentTransform, 
            Dictionary<ModelInfo, GameObject> activeModels)
        {
            //base.BakeModel(adjInfo, parentTransform, ref modelContainer);
            foreach (var model in Models)
            {
                if (model == null) { continue; }
                // Has each defined model evaluate the adjacent cells based on their rules.
                bool isValid = model.Evaluate(adjInfo, this);
                // If this model is valid and is no the currently set model for this RuleModel...
                if (isValid)
                {
                    // If the current model is valid but isnt in the active models dictionary, then that model
                    // game object is created and added.
                    if (!activeModels.ContainsKey(model))
                    {
                        GameObject newModel = Instantiate(model.ModelObject, parentTransform);
                        newModel.transform.localPosition = Vector3.zero;
                        activeModels.Add(model, newModel);
                    }
                }
                else
                {
                    // If the current model is not valid and it exists in the active models dictionary, then it
                    // should be destroyed and removed.
                    if (activeModels.ContainsKey(model))
                    {
                        DestroyImmediate(activeModels[model]);
                        activeModels.Remove(model);
                    }
                }
            }
        }
    }

}