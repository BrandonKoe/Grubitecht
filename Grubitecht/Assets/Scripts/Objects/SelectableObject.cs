/*****************************************************************************
// File Name : SelectableObject.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Allows an object to be selected and provide info about the object.
*****************************************************************************/
using System;
using UnityEngine;

namespace Grubitecht.World.Objects
{
    public class SelectableObject : MonoBehaviour, ISelectable
    {
        public event Action<ISelectable> OnSelectEvent;
        public event Action<ISelectable> OnDeselectEvent;

        public void OnSelect(ISelectable oldObj)
        {
            OnSelectEvent?.Invoke(oldObj);
        }

        public void OnDeselect(ISelectable newObj)
        {
            OnDeselectEvent?.Invoke(newObj);
        }
    }
}