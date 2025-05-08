/*****************************************************************************
// File Name : InfoPanel.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Controls the info panel that presents stats of objects to the player.
*****************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grubitecht.UI.InfoPanel
{
    public class InfoPanelController : MonoBehaviour
    {
        [SerializeField] private Transform infoPanelObject;
        [SerializeField] private Transform infoPanelLayout;
        [Header("Info Displayer Prefabs")]
        [SerializeField] private StringDisplayer stringDisplayer;
        [SerializeField] private NumDisplayer numDisplayer;
        [SerializeField] private SpriteDisplayer spriteDisplayer;

        private readonly List<InfoDisplayerBase> infoDisplayers = new List<InfoDisplayerBase>();
        private static InfoPanelController current;

        /// <summary>
        /// Assigns/De-assigns the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (current != null && current != this)
            {
                Debug.Log("Duplicate InfoPanelController found");
                return;
            }
            else
            {
                current = this;
            }
        }
        private void OnDestroy()
        {
            if (current == this)
            {
                current = null;
            }
        }

        /// <summary>
        /// Shows the info panel and loads a set of info to display to it.
        /// </summary>
        /// <param name="info"></param>
        public static void UpdatePanel(List<InfoValueBase> info)
        { 
            // Load info values here.
            if (current != null)
            {
                // Unload our old info first before loading new info.
                current.UnloadInfo();
                current.LoadInfo(info);
                current.infoPanelObject.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Hides and unloads the info panel.
        /// </summary>
        public static void HidePanel()
        {
            if (current != null)
            {
                current.UnloadInfo();
                current.infoPanelObject.gameObject.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Unloads the info from this info panel.
        /// </summary>
        private void UnloadInfo()
        {
            foreach(InfoDisplayerBase disp in infoDisplayers)
            {
                disp.Unload();
            }
            infoDisplayers.Clear();
        }

        /// <summary>
        /// Loads a set of info to display to this info panel.
        /// </summary>
        /// <param name="info">The list of info values to display.</param>
        private void LoadInfo(List<InfoValueBase> info)
        {
            // Items with the lowest priority will be displayed first.
            info = info.OrderBy(item => item.Priority).ToList();
            // Loop through each value to display and load a displayer for that value.
            foreach(InfoValueBase value in info)
            {
                // Spawn a displayer based on the value type and initialize it with the value.
                switch(value)
                {
                    case StringValue stVal:
                        StringDisplayer stDist = Instantiate(stringDisplayer, infoPanelLayout);
                        infoDisplayers.Add(stDist);
                        stDist.Initialize(stVal);
                        break;
                    case NumValue nVal:
                        NumDisplayer nDist = Instantiate(numDisplayer, infoPanelLayout);
                        nDist.Initialize(nVal);
                        infoDisplayers.Add(nDist);
                        break;
                    case SpriteValue spVal:
                        SpriteDisplayer spDist = Instantiate(spriteDisplayer, infoPanelLayout);
                        spDist.Initialize(spVal);
                        infoDisplayers.Add(spDist);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}