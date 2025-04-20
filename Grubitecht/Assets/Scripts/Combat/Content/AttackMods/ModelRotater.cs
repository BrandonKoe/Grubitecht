/*****************************************************************************
// File Name : ApplyModifier.cs
// Author : Brandon Koederitz
// Creation Date : March 23, 2025
//
// Brief Description : Attacker Mod that applies a modifier to the target attackable when they are attacked.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grubitecht.Combat
{
    public class ModelRotater : AttackerMod
    {
        [SerializeField] private Transform horizontalRotationObject;
        [SerializeField] private Transform verticalRotationObject;

        /// <summary>
        /// Applies all of the listed modifiers to the target when this attacker attacks.
        /// </summary>
        /// <param name="target">The target of the attack.</param>
        protected override void OnAttackPerformed(Attackable target)
        {
            // Get the angle that each object should be facing at to point towards the target
            // Definitely a better way to do this with quaternion math.
            Vector3 directVector = target.transform.position - transform.position;
            Vector2 hVector = new Vector2(directVector.x, directVector.z);
            float hAngle = MathHelpers.VectorToDegAngleWorld(hVector);
            Vector2 vVector = new Vector2(directVector.magnitude, directVector.y);
            float vAngle = MathHelpers.VectorToDegAngle(vVector);

            // Set horizontal and vertical rotations based on the calculated angles.
            Vector3 hEulers = horizontalRotationObject.eulerAngles;
            hEulers.y = hAngle;
            horizontalRotationObject.eulerAngles = hEulers;

            Vector3 vEulers = verticalRotationObject.eulerAngles;
            vEulers.x = vAngle;
            verticalRotationObject.eulerAngles = vEulers;
        }
    }
}