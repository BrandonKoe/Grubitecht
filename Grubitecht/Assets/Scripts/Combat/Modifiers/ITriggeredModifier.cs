/*****************************************************************************
// File Name : ITriggeredModifier.cs
// Author : Brandon Koederitz
// Creation Date : April 13, 2025
//
// Brief Description : Interface for modifiers that trigger when a certain condition happens.
*****************************************************************************/
namespace Grubitecht.Combat
{
    public interface ITriggeredModifier<T> where T : Trigger
    {
        void OnTriggered(T trigger);
    }
}