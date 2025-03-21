/*****************************************************************************
// File Name : DebugBrushEditor.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Custom editor for debug brush.
*****************************************************************************/
using Grubitecht.Tilemaps;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Grubitecht.Editor.Tilemaps
{
    [CustomEditor(typeof(DebugBrush))]
    public class DebugEditor : GridBrushEditorBase
    {
        // Need to have this or else the tilemaps wont show up as valid targets under the tile palette editor.
        public override GameObject[] validTargets
        {
            get
            {
                // Stage handle is basically an editing context.  This method makes it easier to find components.
                StageHandle currentStageHandle = StageUtility.GetCurrentStageHandle();
                return currentStageHandle.FindComponentsOfType<Tilemap>().Where(x =>
                {
                    GameObject gameObject;
                    return (gameObject = x.gameObject).scene.isLoaded
                           && gameObject.activeInHierarchy
                           && !gameObject.hideFlags.HasFlag(HideFlags.NotEditable);
                }).Select(x => x.gameObject).ToArray();
            }
        }
    }
}