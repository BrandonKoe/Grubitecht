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
        private const int NAME_FONT_SIZE = 35;
        #endregion

        [Header("Default Object Information")]
        [SerializeField] protected string objectName;
        [SerializeField, TextArea] protected string objectDesription;

        protected readonly List<InfoValueGetter> infoGetters = new List<InfoValueGetter>();
        public event Action<ISelectable> OnSelectEvent;
        public event Action<ISelectable> OnDeselectEvent;

        #region Properties
        public string ObjectName => objectName;
        public string ObjectDesription => objectDesription;

        public Vector3 Position { get => transform.position; }
        #endregion

        /// <summary>
        /// Get references to all of this object's info getters on awake.
        /// </summary>
        protected virtual void Awake()
        {
            LoadGettersForObject(infoGetters, gameObject);
        }

        /// <summary>
        /// Gets all the info getters from a specific object.
        /// </summary>
        /// <param name="obj">The object to get info getters from.</param>
        /// <returns>The list of found info getters.</returns>
        protected static void LoadGettersForObject(List<InfoValueGetter> getters, GameObject obj)
        {
            IInfoProvider[] providers = obj.GetComponents<IInfoProvider>();
            foreach (var component in providers)
            {
                getters.Add(component.InfoGetter);
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