/*****************************************************************************
// File Name : UIObject.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : A UI object that can be spawned over a position at runtime.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Grubitecht.UI
{
    public abstract class UIObject : MonoBehaviour
    {
        [SerializeField] private EndMode endMode;
        #region Nested
        private enum EndMode
        {
            None,
            Disable,
            Destroy
        }
        #endregion
        /// <summary>
        /// Initializes this object when it spawns on the UI.
        /// </summary>
        public void Initialize()
        {
            StartCoroutine(LifeCycle());
        }

        /// <summary>
        /// Coroutine that controls the life cycle of this object and when it gets destroyed.
        /// </summary>
        /// <returns>Coroutine.</returns>
        protected abstract IEnumerator LifeCycle();

        /// <summary>
        /// Handles what happens at the end of this object's life cycle based on it's endMode.
        /// </summary>
        protected void EndLifeCycle()
        {
            switch(endMode)
            {
                case EndMode.None:
                    break;
                case EndMode.Disable:
                    gameObject.SetActive(false);
                    break;
                case EndMode.Destroy:
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}