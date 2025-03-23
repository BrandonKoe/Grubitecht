/*****************************************************************************
// File Name : TargeterEditor.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Custom editor for the targeter script.
*****************************************************************************/
using UnityEditor;

namespace Grubitecht.Combat.Editors
{
    [CustomEditor(typeof(Targeter))]
    public class TargeterEditor : Editor
    {
        // Allows us to access and modify our GroundBrush object from this editor script.
        public Targeter targeter { get { return target as Targeter; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // Updates the targeter's sphere collider each inspector GUI update so that it is synced with the detection
            // range.
            targeter.UpdateDetectionRange();
        }
    }
}