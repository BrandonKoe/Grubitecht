/*****************************************************************************
// File Name : InfoPopup.cs
// Author : Brandon Koederitz
// Creation Date : May 4, 2025
//
// Brief Description : Allows an object to display a popup with information about that object when the mouse hovers
// over it.
*****************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

namespace Grubitecht.UI
{
    public class InfoPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region CONSTS
        private const string POPUP_PARENT_TAG = "PopupParent";
        #endregion
        [SerializeField, HideInInspector] private PopupDisplayer popupDisplayerPrefab;
        [field: SerializeField] public string TitleText { get; set; }
        [field: SerializeField] public string BodyText { get; set; }

        private PopupDisplayer popupObj;

        private static Transform popupParent;

        #region Properties
        private static Transform PopupParent
        {
            get
            {
                // Finds a popup parent if none exists.
                if (popupParent == null)
                {
                    popupParent = GameObject.FindGameObjectWithTag(POPUP_PARENT_TAG).transform;
                }
                return popupParent;
            }
        }
        #endregion

        /// <summary>
        /// Shows/Hides the popup when the mouse moves over this object.
        /// </summary>
        /// <param name="eventData">Unused.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer Entered");
            SpawnPopup();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            DestroyPopup();
        }

        /// <summary>
        /// Also hide the popup if this object gets disabled while it is shown.
        /// </summary>
        private void OnDisable()
        {
            DestroyPopup();
        }

        /// <summary>
        /// Spawns/Despawns the popup for this object.
        /// </summary>
        private void SpawnPopup()
        {
            popupObj = Instantiate(popupDisplayerPrefab, PopupParent);
            popupObj.Initialize(TitleText, BodyText);
        }
        private void DestroyPopup()
        {
            if (popupObj != null)
            {
                popupObj.Deinitialize();
                Destroy(popupObj.gameObject);
            }
        }
    }
}