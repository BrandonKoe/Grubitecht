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
        private const int NAME_FONT_SIZE = 40;
        #endregion

        [Header("Default Object Information")]
        [SerializeField] private string objectName;
        [SerializeField] private string objectDesription;

        private readonly List<InfoValueGetter> infoGetters = new List<InfoValueGetter>();
        public event Action<ISelectable> OnSelectEvent;
        public event Action<ISelectable> OnDeselectEvent;

        /// <summary>
        /// Get references to all of this object's info getters on awake.
        /// </summary>
        private void Awake()
        {
            IInfoProvider[] providers = GetComponents<IInfoProvider>();
            foreach (var component in providers)
            {
                infoGetters.Add(component.InfoGetter);
            }
        }

        public void OnSelect(ISelectable oldObj)
        {
            OnSelectEvent?.Invoke(oldObj);
        }

        public void OnDeselect(ISelectable newObj)
        {
            OnDeselectEvent?.Invoke(newObj);
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