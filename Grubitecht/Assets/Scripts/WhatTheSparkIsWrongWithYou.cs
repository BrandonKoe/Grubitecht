using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;

public class WhatTheSparkIsWrongWithYou : MonoBehaviour
{
    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
    {
        // Ensures smooth time is always a min value.
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        // This is not the problem, smooth time gets maxed.
        float num = 2f / smoothTime;
        float num2 = num * deltaTime;
        float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
        // Distance from the target to the current angle.
        float value = current - target;
        // Our target angle at the beginning of this function.  Used for determining directions.
        float targetAngleAtFunctionStart = target;

        // Clamps value to be achievable within our smooth time if we're moving at max speed.
        // If our angle is outside the bounds of what is achievable by moving at maxSpeed within
        // smoothTime, then it gets clamped.
        float speedBounds = maxSpeed * smoothTime;
        value = Mathf.Clamp(value, 0f - speedBounds, speedBounds);
        // Target gets reset.  This is mainly for if we've exceeded our speed bounds (which shouldnt be the case
        // by default given max speed is default infinity.
        target = current - value;

        float num6 = (currentVelocity + num * value) * deltaTime;

        currentVelocity = (currentVelocity - num * num6) * num3;
        // The value we should return.
        float newAngle = target + (value + num6) * num3;
        // Right part of the if statement checks for if our new angle has exceeded the target angle based on the direction we're moving determined by the left statement.
        // Left part of the if statement checks for the direction the angle is moving.  True if counterclockwise.
        // This checks if the new angle has exceeded our target angle.  If it has we snap to the target
        // angle and set our current velocity.  
        if (targetAngleAtFunctionStart - current > 0f == newAngle > targetAngleAtFunctionStart)
        {
            Debug.Log("It hit that part");
            newAngle = targetAngleAtFunctionStart;
            // This is the problem since if deltaTime is 0, 
            currentVelocity = (newAngle - targetAngleAtFunctionStart) / deltaTime;
        }

        return newAngle;
    }

    [ExcludeFromDocs]
    public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime)
    {
        float deltaTime = Time.deltaTime;
        float maxSpeed = float.PositiveInfinity;
        return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    }

    public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
    {
        target = current + Mathf.DeltaAngle(current, target);
        return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    }
}
