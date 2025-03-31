/*****************************************************************************
// File Name : InfoDisplayer.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Generic class for components that display a specific type of value on the info panel.
*****************************************************************************/
namespace Grubitecht.UI.InfoPanel
{
    public class InfoDisplayer<T> : InfoDisplayerBase where T : InfoValueBase
    {
        private T value;

        /// <summary>
        /// Initializes this object with the value it should display.
        /// </summary>
        /// <param name="valueRef">The value that should be displayed.</param>
        public virtual void Initialize(T valueRef)
        {
            value = valueRef;
        }
    }
}