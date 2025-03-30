/*****************************************************************************
// File Name : CombatBehaviour.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Root class for components that interact with the game's combat system.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.Combat
{
    public abstract class CombatBehaviour : MonoBehaviour
    {
        #region Component References
        [SerializeReference, HideInInspector] protected Combatant combatant;

        protected virtual void Reset()
        {
            combatant = GetComponent<Combatant>();
        }
        #endregion

        #region Properties
        public Team Team
        {
            get 
            {
                return combatant.CombatTeam;
            }
        }
        public CombatTags Tags
        {
            get
            {
                return combatant.CombatTags;
            }
        }
        #endregion
    }
}
