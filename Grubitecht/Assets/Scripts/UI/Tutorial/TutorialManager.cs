/*****************************************************************************
// File Name : TutorialManager.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Controls the progression of tutorial text.
*****************************************************************************/
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Grubitecht.UI.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TutorialUIObject defaultTutorialObject;
        [SerializeField] private Tutorial[] tutorialProgression;

        private static List<Tutorial> tutorialList;
        private static TutorialUIObject currentTutorial;

        private static TutorialManager instance;

        /// <summary>
        /// Assign/Deassign the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("Duplicate TutorialManger found.");
                return;
            }
            else
            {
                tutorialList = tutorialProgression.ToList();
                instance = this;
                ShowNextTutorial();
            }
        }
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finishedTutorial"></param>
        public static void LogCompleted(Tutorial finishedTutorial)
        {
            if (finishedTutorial != null && finishedTutorial == tutorialList[0])
            {
                tutorialList.Remove(finishedTutorial);
                finishedTutorial.OnTutorialFinished();
                //Debug.Log(currentTutorial);
                // Destroys the current tutorial UI object.
                Destroy(currentTutorial.gameObject);
                currentTutorial = null;

                ShowNextTutorial();
            }
        }

        /// <summary>
        /// Shows the next tutorial in the tutorial sequence.
        /// </summary>
        private static void ShowNextTutorial()
        {
            // If there are no more tutorials to show, then we return.
            if (tutorialList.Count == 0) { return; }
            switch(tutorialList[0].Type)
            {
                case Tutorial.TutorialType.Text:
                    ShowTutorial(tutorialList[0], tutorialList[0].TutorialText);
                    break;
                case Tutorial.TutorialType.GameObject:
                    ShowTutorial(tutorialList[0], tutorialList[0].TutorialPrefab);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Shows a given tutorial for a certain object.
        /// </summary>
        private static void ShowTutorial(Tutorial tutorialObj, string text)
        {
            TutorialUIObject tObj = Instantiate(instance.defaultTutorialObject, instance.transform);
            tObj.TextObject.text = text;
            // Set the dimensions of the tutorial.
            RectTransform trTrans = tObj.transform as RectTransform;
            trTrans.sizeDelta = tutorialObj.TutorialDimensions;
            // Auto-Set the offset of the tutorial 
            tObj.UIOffset = (tutorialObj.TutorialDimensions / 2);
            if (tutorialObj.OverridePosition)
            {
                tObj.Initialize(tutorialObj.TargetPosition);
            }
            else
            {
                tObj.Initialize(tutorialObj);
            }
            currentTutorial = tObj;
            tutorialObj.OnTutorialShown();
        }
        private static void ShowTutorial(Tutorial tutorialObj, TutorialUIObject prefabObject)
        {
            TutorialUIObject tObj = Instantiate(prefabObject, instance.transform);
            if (tutorialObj.OverridePosition)
            {
                tObj.Initialize(tutorialObj.TargetPosition);
            }
            else
            {
                tObj.Initialize(tutorialObj);
            }
            currentTutorial = tObj;
            tutorialObj.OnTutorialShown();
        }
    }
}