/*****************************************************************************
// File Name : StatFormatter.cs
// Author : Brandon Koederitz
// Creation Date : May 4, 2025
//
// Brief Description : A package of information that should be displayed about a stat on the info panel.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.UI.InfoPanel
{
    [CreateAssetMenu(fileName = "StatFormatter", menuName = "Grubitecht/Stat Formatter")]
    public class StatFormatter : ScriptableObject
    {
        [field: SerializeField]public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public string Suffix { get; private set; }
    }
}