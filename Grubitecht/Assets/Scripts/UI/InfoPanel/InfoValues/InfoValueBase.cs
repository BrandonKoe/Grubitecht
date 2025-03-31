/*****************************************************************************
// File Name : InfoValueBase.cs
// Author : Brandon Koederitz
// Creation Date : March 31, 2025
//
// Brief Description : Base class for values that are passed to the info panel to be displayed.  Used to allow for 
// reference passing.
*****************************************************************************/
namespace Grubitecht.UI.InfoPanel
{
    public abstract class InfoValueBase
    {
        // The priority variable is how I will organize the order that these values appear in the info panel.
        public int Priority { get; private set; }

        public InfoValueBase(int priority)
        {
            Priority = priority;
        }
    }
}