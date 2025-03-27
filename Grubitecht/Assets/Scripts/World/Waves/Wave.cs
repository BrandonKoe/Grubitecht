/*****************************************************************************
// File Name : Wave.cs
// Author : Brandon Koederitz
// Creation Date : March 27, 2025
//
// Brief Description : Contains informatoion on a wave of enemies that spawns from a spawn point.
*****************************************************************************/
using Grubitecht.World;
using UnityEngine;

namespace Grubitecht.Waves
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Grubitecht/Wave")]
    public class Wave : ScriptableObject
    {
        [field: SerializeField] public Subwave[] Subwaves { get; private set; }
        #region Nested Classes
        [System.Serializable]
        public class Subwave
        {
            [field: SerializeField] public float Delay { get; private set; }
            [field: SerializeField] public EnemyController[] Enemies { get; private set; }
        }
        #endregion
    }
}