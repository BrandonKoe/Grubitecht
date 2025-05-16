/*****************************************************************************
// File Name : GateObjective.cs
// Author : Brandon Koederitz
// Creation Date : April 12, 2025
//
// Brief Description : Controls the gate objective in level 2.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.World.Objects
{
    public class GateObjective : Objective
    {
        //[SerializeField, Tooltip("The objects that will block the additional spaced taken up by this gate.")]
        //private GridObject[] blockedObjects;
        [Header("Model Change Settings")]
        [SerializeField] private Animator gateAnimator;
        [SerializeField] private GridObject[] dummyObjects;
        [SerializeField] private Component[] removeComponents;
        //[SerializeField,Tooltip("The model for this gate that will get deleted when the gate is destroyed.")] 
        //private GameObject gateModel;

        /// <summary>
        /// Controls the gate being destoryed and burst open.
        /// </summary>
        protected override void OnDeath()
        {
            gateAnimator.SetTrigger("Open");

            /// Destroys all dummy objects so that the way through the gate is clear.
            foreach (GridObject obj in dummyObjects)
            {
                if (obj == null) { continue; }
                Destroy(obj.gameObject);
            }
            /// Destroys all given components on this object that should be removed when this object is destroyed.
            foreach (Component comp in removeComponents)
            {
                if (comp == null) { continue; }
                Destroy(comp);
            }
            base.OnDeath();
            Destroy(this);
        }
    }
}