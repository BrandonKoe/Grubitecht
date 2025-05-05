/*****************************************************************************
// File Name : InfoValue.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Generic class that allows for easier implementation of InfoValues.
*****************************************************************************/
namespace Grubitecht.UI.InfoPanel
{
    public abstract class InfoValue<T> : InfoValueBase
    {
        //#region CONSTS
        //public const float DEFAULT_FONT_SIZE = 22;
        //#endregion
        public T Value { get; set; }

        /// <summary>
        /// All info values should have a base constructor that defines a value.
        /// </summary>
        /// <param name="value">The value to display on the info panel.</param>
        public InfoValue(T value, int priority) : base(priority)
        {
            Value = value;
        }
    }
}