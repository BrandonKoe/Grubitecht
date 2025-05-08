/*****************************************************************************
// File Name : Selector.cs
// Author : Brandon Koederitz
// Creation Date : March 17, 2025
//
// Brief Description : Allows player to select objects in the game world that they click on.
*****************************************************************************/
using Grubitecht.Audio;
using Grubitecht.Tilemaps;
using Grubitecht.UI.InfoPanel;
using Grubitecht.World;
using Grubitecht.World.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Grubitecht
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Camera))]
    public class Selector : MonoBehaviour
    {
        [SerializeField] private SelectionIndicator[] selectionIndicators;
        [SerializeField] private Sound selectSound;
        #region Component References
        [SerializeReference, HideInInspector] private Camera cam;
        #endregion

        public static bool CanInput { get; set; } = true;

        // Input actions
        private InputAction selectAction;
        private InputAction deselectAction;
        private InputAction mousePosAction;

        // Current Selections
        public static ISelectable currentSelection;
        private static SelectionIndicator currentIndicator;

        public static ISelectable CurrentSelection
        {
            get
            {
                return currentSelection;
            }
            set
            {
                // Runn on select and on deselect callbacks, passing in the other object.
                if (currentSelection != null)
                {
                    currentSelection.OnDeselect(value);
                }
                if (value != null)
                {
                    value.OnSelect(currentSelection);

                }

                // If the thing being selected is a selectable object, then we should update the info panel with
                // info about it's values.
                if (value is SelectableObject sObj)
                {
                    Debug.Log("Updating info panel values");
                    InfoPanelController.UpdatePanel(sObj.GetInfoValues());
                }
                else
                {
                    InfoPanelController.HidePanel();
                }
                currentSelection = value;
            }
        }

        /// <summary>
        /// Assign component references on component reset.
        /// </summary>
        private void Reset()
        {
            cam = GetComponent<Camera>();
        }

        /// <summary>
        /// Setup player input.
        /// </summary>
        private void Awake()
        {
            if (TryGetComponent(out PlayerInput input))
            {
                selectAction = input.currentActionMap.FindAction("Select");
                deselectAction = input.currentActionMap.FindAction("Deselect");
                mousePosAction = input.currentActionMap.FindAction("MousePos");                                        

                selectAction.performed += SelectAction_Performed;
                deselectAction.performed += DeselectAction_Performed;
                CanInput = true;
            }
        }

        /// <summary>
        /// Unsubscribe events.
        /// </summary>
        private void OnDestroy()
        {
            selectAction.performed -= SelectAction_Performed;
            deselectAction.performed -= DeselectAction_Performed;
        }

        /// <summary>
        /// Handles players inputting a selection click on their screen.
        /// </summary>
        /// <param name="obj">unused.</param>
        private void SelectAction_Performed(InputAction.CallbackContext obj)
        {
            if (!CanInput) { return; }
            // Players cannot select if the level is not playing.
            if (!LevelManager.IsPlaying) { return; }
            ISelectable clicked = GetSelectableAtMousePos();
            Debug.Log("Select input registered" + clicked);
            // If the currently selected object is clicked, then it is deselected.
            if (clicked == CurrentSelection)
            {
                CurrentSelection = null;
            }
            else
            {
                if (clicked != null)
                {
                    AudioManager.PlaySoundAtPosition(selectSound, clicked.Position);
                }
                CurrentSelection = clicked;
            }

            UpdateSelectionIndicator();
        }

        /// <summary>
        /// Deselect the current object if the player inputs a right click.
        /// </summary>
        /// <param name="obj"></param>
        private void DeselectAction_Performed(InputAction.CallbackContext obj)
        {
            if (!CanInput) { return; }
            CurrentSelection = null;
            UpdateSelectionIndicator();
        }

        /// <summary>
        /// Updates the selection indicator to communicate to the player what is currently selected.
        /// </summary>
        private void UpdateSelectionIndicator()
        {
            // Disables the current indicator.
            if (currentIndicator != null)
            {
                currentIndicator.Disable();
                currentIndicator = null;
            }
            if (CurrentSelection != null)
            {
                // Gets an indicator that is coded to handle the specific type of component specified.
                // This lets me design multiple indicators that behave differently for different types of selectable
                // objects.
                SelectionIndicator indicator = Array.Find(selectionIndicators, item =>
                    item.SelectedComponentTypes.Contains(CurrentSelection.GetType()));
                if (indicator != null)
                {
                    indicator.Enable();
                    indicator.IndicateSelected(CurrentSelection);
                    currentIndicator = indicator;
                }
            }
        }

        /// <summary>
        /// Gets a selectable object at the mouse's position on the screen.
        /// </summary>
        /// <returns>The selecteable object at that screen position.</returns>
        private ISelectable GetSelectableAtMousePos()
        {
            return GetSelectableAtScreenPos(mousePosAction.ReadValue<Vector2>(), cam);
        }

        /// <summary>
        /// Gets a selectable object at a given position on the screen.
        /// </summary>
        /// <returns>The selecteable object at that screen position.</returns>
        private static ISelectable GetSelectableAtScreenPos(Vector2 screenPos, Camera cam)
        {
            // Check for UI selections
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = screenPos;
            List<RaycastResult> uiResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, uiResults);
            // If the player clicks on a UI selectable object, they should ignore anything beneath it.
            foreach(var result in uiResults)
            {
                Debug.Log("Checking result");
                if (result.gameObject.TryGetComponent(out ISelectable uiSelection))
                {
                    return uiSelection;
                }
            }

            // Check for world point selections.
            Ray selectionRay = cam.ScreenPointToRay(screenPos);

            if (Physics.Raycast(selectionRay, out RaycastHit results))
            {
                // Attempts to get a component that implements the ISelectable interface.  If none is found on the
                // clicked object then null is returned instead.
                if (results.collider.gameObject.TryGetComponent(out ISelectable selectable))
                {
                    if (selectable is SelectableChunk selectedChunk)
                    {
                        VoxelTilemap3D tilemap = selectedChunk.Chunk.Tilemap;
                        Vector3Int gridPos = tilemap.WorldToGridPos(results.point);
                        VoxelTile tile = tilemap.GetTile((Vector2Int)gridPos);
                        // Gets the closest space to location the player clicked.
                        return new SpaceSelection(tile, tilemap.GridToWorldPos(tile.GridPosition));
                    }
                    return selectable;
                }
            }
            return null;
        }
    }

}