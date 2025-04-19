/*****************************************************************************
// File Name : Injected.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Debuff effect that will cause damage after a set amount of time and will spawn a new tiphia 
youngling.
*****************************************************************************/
using Grubitecht.Tilemaps;
using Grubitecht.World;
using Grubitecht.World.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    [CreateAssetMenu(fileName = "Injected", menuName = "Grubitecht/Modifiers/Injected")]
    public class Injected : DurationModifier<Attackable>
    {
        [Header("Injected Settings")]
        [SerializeField] internal EnemyController tiphiaYounglingPrefab;
        [SerializeField] private int damageAmount;

        protected override void OnModifierExpired(Attackable appliedBehaviour)
        {
            base.OnModifierExpired(appliedBehaviour);
            appliedBehaviour.TakeDamage(damageAmount);
            EnemyController.SpawnEnemy(tiphiaYounglingPrefab,
                VoxelTilemap3D.Main_GetApproximateSpace(appliedBehaviour.transform.position));
        }
    }
}
