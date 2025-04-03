/*****************************************************************************
// File Name : IInfoProvider.cs
// Author : Brandon Koederitz
// Creation Date : April 2nd, 2025
//
// Brief Description : Interface that allows component of amn object to provide information that is shown in the info 
panel.
*****************************************************************************/
namespace Grubitecht.UI.InfoPanel
{
    public interface IInfoProvider
    {
        InfoValueBase[] InfoGetter();
    }
}