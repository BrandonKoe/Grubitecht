/*****************************************************************************
// File Name : WorldSpaceCanvasManager.cs
// Author : Brandon Koederitz
// Creation Date : March 29, 2025
//
// Brief Description : Manages visualizing UI objects that exist in the world at runtime.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.UI
{
    public class WorldSpaceCanvasManager : MonoBehaviour
    {
        private static WorldSpaceCanvasManager instance;

        /// <summary>
        /// Assigns the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("Multiple CanvasManagers found.");
                return;
            }
            else
            {
                instance = this;
            }
        }
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Shows a given UI object on this canvas.
        /// </summary>
        /// <typeparam name="T">The type of UI object to display.</typeparam>
        /// <param name="prefab">The prefab of the UI object to spawn.</param>
        /// <param name="position">The position on the world to display this object over.</param>
        /// <returns>The created instance of the object.</returns>
        public static T ShowUIObject<T>(T prefab, Vector3 position) where T : UIObject
        {
            T inst = Instantiate(prefab, instance.transform);
            inst.transform.position = position;
            Vector3 pos = inst.transform.localPosition;
            pos.z = 0;
            inst.transform.localPosition = pos;
            inst.Initialize();
            return inst;
        }
    }
}