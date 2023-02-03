using System.Collections.Generic;
using System.Linq;
using AppConfiguration.ManagerUI.Configuration;

namespace AppConfiguration.ManagerUI.Core.Extensions
{
    internal static class UiResourceExtensions
    {
        public static UiResource GetMainUi(this IEnumerable<UiResource> resources, MapAppConfigurationOptions options)
        {
            var resource = resources
                .FirstOrDefault(r => r.ContentType == ContentType.Html && r.FileName == Constants.APPCONFIGURATION_UI_MAIN_RESOURCE);

            if (resource is null) return null;

            var cssResource = resources
                .FirstOrDefault(r => r.ContentType == ContentType.Css && r.FileName == Constants.APPCONFIGURATION_UI_CSS_RESOURCE);

            resource.Content = resource.Content.Replace(Constants.APPCONFIGURATION_UI_CSS_TARGET, cssResource.Content);

            var vendorsResource = resources
                .FirstOrDefault(r => r.ContentType == ContentType.Javascript && r.FileName == Constants.APPCONFIGURATION_UI_VENDORS_RESOURCE);

            resource.Content = resource.Content.Replace(Constants.APPCONFIGURATION_UI_VENDORS_TARGET, vendorsResource.Content);

            var bundleResource = resources
                .FirstOrDefault(r => r.ContentType == ContentType.Javascript && r.FileName == Constants.APPCONFIGURATION_UI_BUNDLES_RESOURCE);

            resource.Content = resource.Content.Replace(Constants.APPCONFIGURATION_UI_BUNDLES_TARGET, bundleResource.Content);

            resource.Content = resource.Content.Replace(Constants.APPCONFIGURARION_UI_API_NAMESPACE, options.ApiNamespace);

            return resource;
        }
    }
}
