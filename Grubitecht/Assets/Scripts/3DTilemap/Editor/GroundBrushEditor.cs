/*****************************************************************************
// File Name : GroundBrushEditor.cs
// Author : Brandon Koederitz
// Creation Date : March 6, 2025
//
// Brief Description : custom editor for a 3D ground brush.
*****************************************************************************/
using Grubitecht.Tilemaps;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace Grubitecht.Editor.Tilemaps
{
    [CustomEditor(typeof(GroundBrush))]
    public class GroundBrushEditor : GridBrushEditorBase
    {
        // Need to have this or else the tilemaps wont show up as valid targets under the tile palette editor.
        public override GameObject[] validTargets
        {
            get
            {
                // Stage handle is basically an editing context.  This method makes it easier to find components.
                StageHandle currentStageHandle = StageUtility.GetCurrentStageHandle();
                return currentStageHandle.FindComponentsOfType<Tilemap3DLayer>().Where(x =>
                {
                    GameObject gameObject;
                    return (gameObject = x.gameObject).scene.isLoaded
                           && gameObject.activeInHierarchy
                           && !gameObject.hideFlags.HasFlag(HideFlags.NotEditable)
                           && x.RootTilemap != null;
                }).Select(x => x.gameObject).ToArray();
            }
        }

        // Allows us to access and modify our GroundBrush object from this editor script.
        public GroundBrush targetBrush {  get { return target as GroundBrush; } }

        /// <summary>
        /// Changes whether or not we should place vales half a cell higher than normal to allow for easier painting of stepping stones.
        /// </summary>
        /// <param name="gridLayout">Unused.</param>
        /// <param name="brushTarget">Unused.</param>
        /// <param name="position">Unused.</param>
        /// <param name="tool">Unused.</param>
        /// <param name="executing">Unused.</param>
        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, 
            GridBrushBase.Tool tool, bool executing)
        {
            Event currentEvent = Event.current;
            // Do not know if there is a new input system equivalent for this.  Will research.
            if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Space)
            {
                targetBrush.HalfStepPlacement = !targetBrush.HalfStepPlacement;
            }
            base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
        }
    }
}
