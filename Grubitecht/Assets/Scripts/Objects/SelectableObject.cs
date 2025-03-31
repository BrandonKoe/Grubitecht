/*****************************************************************************
// File Name : SelectableObject.cs
// Author : Brandon Koederitz
// Creation Date : March 25, 2025
//
// Brief Description : Allows an object to be selected and provide info about the object.
*****************************************************************************/
using Grubitecht.UI.InfoPanel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.World.Objects
{
    public delegate InfoValueBase[] InfoValueGetter();
    public class SelectableObject : MonoBehaviour, ISelectable
    {
        #region CONSTS
        private const int NAME_FONT_SIZE = 50;
        #endregion

        [Header("Default Object Information")]
        [SerializeField] private string objectName;
        [SerializeField] private string objectDesription;

        private readonly List<InfoValueGetter> infoGetters = new List<InfoValueGetter>();
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

        /// <summary>
        /// Adds/removes an info getter that this component will use to get the info to display on the info panel'
        /// when this object is selected.
        /// </summary>
        public void AddInfoGetter(InfoValueGetter getter)
        {
            infoGetters.Add(getter);
        }
        public void RemoveInfoGetter(InfoValueGetter getter)
        {
            infoGetters.Remove(getter);
        }

        /// <summary>
        /// Gets the info values to display on the panel for this selected object.
        /// </summary>
        /// <returns>The list of info values that should be displayed on the info panel.</returns>
        public List<InfoValueBase> GetInfoValues()
        {
            // Sets up the list of values along with the default values that should be displayed for all selectable
            // objects.
            List<InfoValueBase> values = new List<InfoValueBase>()
            {
                new StringValue(objectName, 0, NAME_FONT_SIZE),
            };
            // Get info values from other components here.
            foreach (InfoValueGetter getter in infoGetters)
            {
                values.AddRange(getter());
            }
            values.Add(new StringValue(objectDesription, 100));
            return values;
        }
    }
}