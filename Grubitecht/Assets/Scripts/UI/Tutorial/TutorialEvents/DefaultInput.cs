/*****************************************************************************
// File Name : TutorialEvent.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Controls the events that trigger a tutorial as finished.
*****************************************************************************/
using UnityEngine;

namespace Grubitecht.UI.Tutorial
{
    [CreateAssetMenu(fileName = "DefaultInput", menuName = "Grubitecht/TutorialEvents/Basic Input")]
    public class DefaultInput : TutorialEvent
    {
        public override void Initialize(TutorializedObject obj)
        {
            TutorialInputController.OnConfirmTutorialEvent += obj.CompleteTutorial;
        }

        public override void Deinitialize(TutorializedObject obj)
        {
            TutorialInputController.OnConfirmTutorialEvent -= obj.CompleteTutorial;
        }
    }
}