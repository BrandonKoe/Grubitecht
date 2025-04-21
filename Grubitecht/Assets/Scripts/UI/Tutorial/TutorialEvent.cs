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
    public abstract class TutorialEvent : ScriptableObject
    {
        public abstract void Initialize(Tutorial obj);

        public abstract void Deinitialize(Tutorial obj);
    }
}