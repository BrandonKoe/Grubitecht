/*****************************************************************************
// File Name : CombatBehaviour.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Root class for components that interact with the game's combat system.
*****************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public abstract class CombatBehaviour : MonoBehaviour
    {
        #region Component References
        [SerializeReference, HideInInspector] protected Combatant combatant;
        [SerializeReference, HideInInspector] protected Attackable attackable;

        protected virtual void Reset()
        {
            combatant = GetComponent<Combatant>();
            attackable = GetComponent<Attackable>();
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

        /// <summary>
        /// Subscribe/Unsubscribe the OnDeath broadcast.
        /// </summary>
        protected virtual void Awake()
        {
            if (attackable != null)
            {
                attackable.OnDeath += BroadcastDeath;
            }
        }
        protected virtual void OnDestroy()
        {
            if (attackable != null)
            {
                attackable.OnDeath -= BroadcastDeath;
            }
        }

        /// <summary>
        /// Broadcasts out to any listeners that the object attached to this combat behaviour has died.  Used to
        /// remove references.
        /// </summary>
        protected virtual void BroadcastDeath() { }
    }
}
