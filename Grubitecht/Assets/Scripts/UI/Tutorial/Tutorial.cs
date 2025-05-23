/*****************************************************************************
// File Name : TutorializedObject.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Controls tuturoails that pop up for a given object.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using Grubitecht.World.Objects;
using Grubitecht.Tilemaps;

namespace Grubitecht.UI.Tutorial
{
    [System.Serializable]
    public class Tutorial
    {
        [SerializeField] private GameObject targetObject;
        [field: SerializeField] private bool targetChild;
        [SerializeField, ShowIf("TargetChild"), AllowNesting] private int childIndex;
        [SerializeField] private TutorialType type;
        [SerializeField, ShowIf("type", TutorialType.Text), AllowNesting, TextArea] 
        private string tutorialText;
        [SerializeField, ShowIf("type", TutorialType.Text), AllowNesting]
        private Vector2 tutorialDimensions;
        [SerializeField, ShowIf("type", TutorialType.Text), AllowNesting]
        private Vector2 tutorialOffset;
        [SerializeField, ShowIf("type", TutorialType.GameObject), AllowNesting] 
        private TutorialUIObject tutorialPrefab;

        [SerializeField] private TutorialEvent finishEvent;

        [Header("Events")]
        [SerializeField] private UnityEvent OnTutorialShownEvent;
        [SerializeField] private UnityEvent OnTutorialFinishedEvent;


        #region Properties
        public GameObject TargetObject => targetObject;
        public TutorialType Type => type;
        public string TutorialText => tutorialText;
        public Vector2 TutorialDimensions => tutorialDimensions;
        public Vector2 TutorialOffset => tutorialOffset;
        public TutorialUIObject TutorialPrefab => tutorialPrefab;
        public TutorialEvent FinishEvent => finishEvent;
        public bool TargetChild => targetChild;
        #endregion


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
            OnTutorialShownEvent?.Invoke();
        }
        public virtual void OnTutorialFinished()
        {
            finishEvent.Deinitialize(this);
            OnTutorialFinishedEvent?.Invoke();
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

        /// <summary>
        /// Logs this tutorial as completed so the player can move to the next tutorial.
        /// </summary>
        public void CompleteTutorial(MovableObject obj, VoxelTile targetTile)
        {
            if (finishEvent is OnMove onMove)
            {
                if (targetTile.GridPosition2 == onMove.TargetSpace)
                {
                    TutorialManager.LogCompleted(this);
                }
            }
            else
            {
                TutorialManager.LogCompleted(this);
            }
        }

        /// <summary>
        /// Has this tutorial find an object if it's marked SearchForGameObjectOfName
        /// </summary>
        public void FindObject()
        {
            if (TargetChild && childIndex < targetObject.transform.childCount)
            {
                targetObject = targetObject.transform.GetChild(childIndex).gameObject;
            }
        }
    }
}
