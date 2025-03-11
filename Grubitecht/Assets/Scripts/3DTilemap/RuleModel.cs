/*****************************************************************************
// File Name : RuleModel.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Controls the composite model of this object that adapts to the placement of other similar
// rule models around it.
*****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    public class RuleModel : MonoBehaviour
    {
        [SerializeField] private Transform modelContainer;
        [field: SerializeField] public RuleTile3D RuleTile { get; private set; }

        // Composite model settings
        private readonly Dictionary<RuleTile3D.ModelInfo, GameObject> compositeModels = new();

        // Non-Composite model settings
        private RuleTile3D.ModelInfo dominantModel;
        private GameObject modelObject;

        /// <summary>
        /// Updates this rule model based on the information of it's RuleTile3D and the passed in adjacent cell info.
        /// </summary>
        /// <param name="info">Info about the cells adjacent to this one.</param>
        public void UpdateRuleModel(AdjacentCellInfo info)
        {
            foreach (var model in RuleTile.Models)
            {
                if (model == null) { continue; }
                
                // Has the rule tile evaluate the passed in adjacent cell info based on the rules for a given model.
                bool isValid = model.Evaluate(info, RuleTile);
                // Update the model here.
                // If the model should be composite, then we check it against the currently present models and add
                // it if it doesnt already exist.
                if (RuleTile.IsComposite)
                {
                    if (isValid)
                    {
                        // Add the model
                        if (!compositeModels.ContainsKey(model))
                        {
                            GameObject newModel = Instantiate(model.ModelObject, modelContainer);
                            newModel.transform.localPosition = Vector3.zero;
                            compositeModels.Add(model, newModel);
                        }
                    }
                    else
                    {
                        // Remove the model
                        if (compositeModels.ContainsKey(model))
                        {
                            DestroyImmediate(compositeModels[model]);
                            compositeModels.Remove(model);
                        }
                    }
                }
                // If the model is not composite, then we simply override the current model.
                else
                {
                    if (isValid && model != dominantModel)
                    {
                        DestroyImmediate(modelObject);
                        modelObject = Instantiate(model.ModelObject, modelContainer);
                        modelObject.transform.localPosition = Vector3.zero;
                        dominantModel = model;
                    }
                    break;
                }
            }
        }
    } 
}