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
    [CreateAssetMenu(fileName = "OnWave", menuName = "Grubitecht/TutorialEvents/On Wave")]
    public class OnWave : TutorialEvent
    {
        public override void Initialize(Tutorial obj)
        {
            SpawnPoint sPoint = obj.TargetObject.GetComponent<SpawnPoint>();
            sPoint.OnSpawnWave += obj.CompleteTutorial;
        }

        public override void Deinitialize(Tutorial obj)
        {
            SpawnPoint sPoint = obj.TargetObject.GetComponent<SpawnPoint>();
            sPoint.OnSpawnWave -= obj.CompleteTutorial;
        }
    }
}