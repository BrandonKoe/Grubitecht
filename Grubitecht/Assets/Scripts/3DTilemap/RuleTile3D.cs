/*****************************************************************************
// File Name : RuleTileThree.cs
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
        [SerializeField] private bool isComposite;
        [SerializeField] private ModelInfo[] models;
        #endregion

        #region Nested Classes
        [System.Serializable]
        private class ModelInfo
        {
            [SerializeField] private GameObject modelObject;
            [SerializeField] private Rule[] rules;
        }

        [System.Serializable]
        private class Rule
        {
            [SerializeField] private Vector3 rulingPosition;
            [SerializeField] private RuleType ruleType;

            public bool ApplyRule()
            {
                bool result = false;
                switch (ruleType)
                {
                    case RuleType.Ignore:
                        result = true;
                        break;
                    case RuleType.None:
                        break;
                    case RuleType.Any:
                        break;
                    case RuleType.Exclusive:
                        break;
                    case RuleType.NotExclusive:
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