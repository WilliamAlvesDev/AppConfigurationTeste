using System.Collections.Generic;

namespace AppConfiguration.ManagerUI.Core
{
    internal interface IUiResourcesReader
    {
        IEnumerable<UiResource> UiResources { get; }
    }
}
