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
        [SerializeField] private GameObject openGatePrefab;
        //[SerializeField,Tooltip("The model for this gate that will get deleted when the gate is destroyed.")] 
        //private GameObject gateModel;

        /// <summary>
        /// Controls the gate being destoryed and burst open.
        /// </summary>
        protected override void OnDeath()
        {
            //// Destroy all objects that take up extra space for the gate so that other objects can now move through it.
            //foreach (var obj in blockedObjects)
            //{
            //    if (obj == null) { continue; }
            //    Destroy(obj.gameObject);
            //}
            //// Destroy the model of the closed gate and spawn the open gate model
            //if (gateModel != null)
            //{
            //    Transform modelParent = gateModel.transform.parent;
            //    Destroy(gateModel);
            //    Instantiate(openGatePrefab, modelParent);
            //}

            // Instantiates the prefab for the object that will show the broken open gate once the gate is destroyed.
            Instantiate(openGatePrefab, transform.position, transform.rotation);
            base.OnDeath();
        }
    }
}