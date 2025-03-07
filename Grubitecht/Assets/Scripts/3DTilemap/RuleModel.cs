/*****************************************************************************
// File Name : RuleModel.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : Controls the composite model of this object that adapts to the placement of other similar
// rule models around it.
*****************************************************************************/

using UnityEngine;

namespace Grubitecht.Tilemaps
{
    public class RuleModel : MonoBehaviour
    {
        [SerializeField] private Transform modelContainer;
        [SerializeField] private RuleTile3D info;
    } 

}