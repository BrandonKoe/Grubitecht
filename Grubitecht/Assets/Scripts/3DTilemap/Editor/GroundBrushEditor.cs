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
    }
}
