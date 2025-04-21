/*****************************************************************************
// File Name : TutorialEvent.cs
// Author : Brandon Koederitz
// Creation Date : March 20, 2025
//
// Brief Description : Controls the events that trigger a tutorial as finished.
*****************************************************************************/
using Grubitecht.Waves;
using Grubitecht.World.Objects;
using UnityEngine;

namespace Grubitecht.UI.Tutorial
{
    [CreateAssetMenu(fileName = "OnWaveFinished", menuName = "Grubitecht/TutorialEvents/On Wave Finished")]
    public class OnWaveFinished : TutorialEvent
    {
        public override void Initialize(Tutorial obj)
        {
            WaveManager sPoint = obj.TargetObject.GetComponent<WaveManager>();
            sPoint.OnFinishWave += obj.CompleteTutorial;
        }

        public override void Deinitialize(Tutorial obj)
        {
            WaveManager sPoint = obj.TargetObject.GetComponent<WaveManager>();
            sPoint.OnFinishWave -= obj.CompleteTutorial;
        }
    }
}