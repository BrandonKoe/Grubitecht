/*****************************************************************************
// File Name : TutorializedObject.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Controls tuturoails that pop up for a given object.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Events;

namespace Grubitecht.UI.Tutorial
{
    public class TutorializedObject : MonoBehaviour
    {
        [field: SerializeField] public TutorialType Type { get; private set; }
        [field: SerializeField] public string TutorialText { get; private set; }
        [field: SerializeField] public TutorialUIObject TutorialPrefab { get; private set; }
        [field: SerializeField] public bool OverridePosition { get; private set; }
        [field: SerializeField] public Vector3 TargetPosition { get; private set; }
        [SerializeField] private TutorialEvent finishEvent;

        [Header("Events")]
        [SerializeField] private UnityEvent OnTutorialShownEvent;
        [SerializeField] private UnityEvent OnTutorialFinishedEvent;

        #region Nested
        public enum TutorialType
        {
            Text,
            GameObject
        }
        #endregion

        /// <summary>
        /// Initializes/Deinitializes the event that flags this ttutorial as completed.
        /// </summary>
        public virtual void OnTutorialShown()
        {
            finishEvent.Initialize(this);
        }
        public virtual void OnTutorialFinished()
        {
            finishEvent.Deinitialize(this);
        }

        /// <summary>
        /// Logs this tutorial as completed so the player can move to the next tutorial.
        /// </summary>
        public void CompleteTutorial()
        {
            TutorialManager.LogCompleted(this);
        }

        /// <summary>
        /// Logs this tutorial as completed so the player can move to the next tutorial.
        /// </summary>
        public void CompleteTutorial(ISelectable selectableDummy)
        {
            TutorialManager.LogCompleted(this);
        }
    }
}
