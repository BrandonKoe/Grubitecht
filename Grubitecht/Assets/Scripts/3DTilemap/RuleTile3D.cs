/*****************************************************************************
// File Name : RuleTile3D.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Set of model information for creating ground tilemaps in 3D space.
*****************************************************************************/
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grubitecht.Tilemaps
{
    [CreateAssetMenu(fileName = "RuleTile3D", menuName = "Tilemap3D/Rule Tile 3D")]
    public class RuleTile3D : ScriptableObject
    {
        #region vars
        [field: SerializeField] public bool IsComposite { get; private set; }
        [field: SerializeField] public ModelInfo[] Models { get; private set; }
        #endregion

        #region Nested Classes
        [System.Serializable]
        public class ModelInfo
        {
            [field: SerializeField] public GameObject ModelObject { get; private set; }
            [SerializeField] private Rule[] rules;

            public bool Evaluate(AdjacentCellInfo info, RuleTile3D ruleTile)
            {
                bool antiResult = false;
                foreach (var rule in rules)
                {
                    // Loops through each rule and checks if it should apply.  Uses OR condition and the opposite of
                    // if the rule should apply so that if any rule does not apply, then antiResult will be set to true
                    antiResult |= !rule.ApplyRule(info, ruleTile);
                }
                return !antiResult;
            }
        }

        [System.Serializable]
        private class Rule
        {
            [SerializeField] private Vector3Int rulingPosition;
            [SerializeField] private RuleType ruleType;

            public bool ApplyRule(AdjacentCellInfo info, RuleTile3D ruleTile)
            {
                rulingPosition.ClampVector(-1, 1);
                // Gets the tile corresponding to this rule from the adjacent cell info.  If it doesnt exist, then
                // the tile should be null;
                Tile3D tile = null;
                if (info.AdjacentTiles.ContainsKey(rulingPosition))
                {
                    tile = info.AdjacentTiles[rulingPosition];
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
                            // If the adjacent tile does not have a rule model, then it is treated as empty.
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
                            result = false;
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
    }
}