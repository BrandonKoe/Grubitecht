/*****************************************************************************
// File Name : RuleTile3D.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Set of model information for creating ground tilemaps in 3D space.
*****************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.Tilemaps
{
    [CreateAssetMenu(fileName = "RuleTile3D", menuName = "Tilemap3D/Rule Tile 3D")]
    public class RuleTile3D : ScriptableObject
    {
        #region vars
        [field: SerializeField] public ModelInfo[] Models { get; private set; }
        #endregion

        #region Nested Classes
        [System.Serializable]
        public class ModelInfo
        {
            [field: SerializeField] public GameObject ModelObject { get; private set; }
            [SerializeField] private Rule[] rules;

            /// <summary>
            /// Evalutes whether this model is valid or not based on information about adjacent tiles and the rules
            /// of this model.
            /// </summary>
            /// <param name="adjInfo">Info about adjacent tiles.</param>
            /// <param name="ruleTile">The rule tile this model belongs to.</param>
            /// <returns>Whether this model is valid to be shown or not.</returns>
            public bool Evaluate(Dictionary<Vector3, Tile3D> adjInfo, RuleTile3D ruleTile)
            {
                bool antiResult = false;
                foreach (var rule in rules)
                {
                    // Loops through each rule and checks if it should apply.  Uses OR condition and the opposite of
                    // if the rule should apply so that if any rule does not apply, then antiResult will be set to true
                    antiResult |= !rule.ApplyRule(adjInfo, ruleTile);
                }
                return !antiResult;
            }
        }

        [System.Serializable]
        private class Rule
        {
            [SerializeField] internal Vector3Int rulingPosition;
            [SerializeField] private RuleType ruleType;

            /// <summary>
            /// Evaluates the results of this rule given a set of adjacent tile info.
            /// </summary>
            /// <param name="adjInfo">Info about the tiles adjacent to this evaluated tile.</param>
            /// <param name="ruleTile">The rule tile that this rule belongs to.</param>
            /// <returns>True if the rule is met, false if it is not met.</returns>
            public bool ApplyRule(Dictionary<Vector3, Tile3D> adjInfo, RuleTile3D ruleTile)
            {
                rulingPosition.ClampVector(-1, 1);
                Tile3D tile;
                if (adjInfo.ContainsKey(rulingPosition))
                {
                    tile = adjInfo[rulingPosition];
                }
                else
                {
                    // If adjInfo lacks any information about the tile this rule evaluates, then it is treated as
                    // empty.
                    tile = null;
                }

                bool result = false;
                switch (ruleType)
                {
                    case RuleType.Ignore:
                        result = true;
                        break;
                    case RuleType.None:
                        result = tile == null;
                        break;
                    case RuleType.Any:
                        result = tile != null;
                        break;
                    case RuleType.Exclusive:
                        if (tile.RuleModel != null)
                        {
                            result = tile.RuleModel.RuleTile == ruleTile;
                        }
                        else
                        {
                            // If the adjacent tile does not have a rule model, then it is treated as a tile that does
                            // not share this rule tile.
                            result = false;
                        }
                        break;
                    case RuleType.NotExclusive:
                        if (tile.RuleModel != null)
                        {
                            result = tile.RuleModel.RuleTile != ruleTile;
                        }
                        else
                        {
                            result = true;
                        }
                        break;
                    default:
                        result = true;
                        break;
                }
                return result;
            }
        }

        private enum RuleType
        {
            Ignore,
            None,
            Any,
            Exclusive,
            NotExclusive
        }
        #endregion

        /// <summary>
        /// Recreates the model for a RuleModel object based on it's adjacent tile info.
        /// </summary>
        /// <param name="adjInfo">Info about the tiles adjacent to the tile to change the model of.</param>
        /// <param name="parentTransform">The transform that will contain the model.</param>
        /// <param name="activeModel">The model that the rule model component is actively using.</param>
        public virtual void BakeModel(Dictionary<Vector3, Tile3D> adjInfo, Transform parentTransform, 
            Dictionary<ModelInfo, GameObject> activeModel)
        {
            foreach (var model in Models)
            {
                if (model == null) { continue; }
                // Has each defined model evaluate the adjacent cells based on their rules.
                bool isValid = model.Evaluate(adjInfo, this);
                // If this model is valid and is no the currently set model for this RuleModel...
                if (isValid && !activeModel.ContainsKey(model))
                {
                    // Destroy the old model game object.
                    GameObject currentObj = activeModel.Values.Single();
                    DestroyImmediate(currentObj);
                    // Create the new model game object.
                    GameObject newModelObj = Instantiate(model.ModelObject, parentTransform);
                    newModelObj.transform.localPosition = Vector3.zero;
                    // Set the rule model's reference to this newly created model.
                    activeModel.Clear();
                    activeModel.Add(model, newModelObj);
                    break;
                }
            }
        }
    }
}