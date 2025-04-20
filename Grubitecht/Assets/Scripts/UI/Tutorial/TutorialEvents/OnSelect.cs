/*****************************************************************************
// File Name : TutorialEvent.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Controls the events that trigger a tutorial as finished.
*****************************************************************************/
using Grubitecht.World.Objects;
using UnityEngine;

namespace Grubitecht.UI.Tutorial
{
    [CreateAssetMenu(fileName = "OnSelect", menuName = "Grubitecht/TutorialEvents/On Select")]
    public class OnSelect : TutorialEvent
    {
        public override void Initialize(TutorializedObject obj)
        {
            SelectableObject selComp = obj.GetComponent<SelectableObject>();
            selComp.OnSelectEvent += obj.CompleteTutorial;
        }

        public override void Deinitialize(TutorializedObject obj)
        {
            SelectableObject selComp = obj.GetComponent<SelectableObject>();
            selComp.OnSelectEvent -= obj.CompleteTutorial;
        }
    }
}