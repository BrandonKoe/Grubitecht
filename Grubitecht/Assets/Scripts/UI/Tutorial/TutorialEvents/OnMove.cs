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
    [CreateAssetMenu(fileName = "OnMove", menuName = "Grubitecht/TutorialEvents/On Move")]
    public class OnMove : TutorialEvent
    {
        public override void Initialize(Tutorial obj)
        {
            GridObject gObj = obj.TargetObject.GetComponent<GridObject>();
            gObj.OnChangeSpace += obj.CompleteTutorial;
        }

        public override void Deinitialize(Tutorial obj)
        {
            GridObject gObj = obj.TargetObject.GetComponent<GridObject>();
            gObj.OnChangeSpace -= obj.CompleteTutorial;
        }
    }
}