using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AppConfiguration.ManagerUI.Core
{
    internal class UiEmbeddedResourcesReader : IUiResourcesReader
    {
        private readonly Assembly _assembly;

        public UiEmbeddedResourcesReader(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public IEnumerable<UiResource> UiResources
        {
            get
            {
                var embeddedResources = _assembly.GetManifestResourceNames();

                return ParseEmbeddedResources(embeddedResources);
            }
        }

        private IEnumerable<UiResource> ParseEmbeddedResources(IEnumerable<string> embeddedFiles)
        {
            const char splitSeparator = '.';

            var resourceList = new List<UiResource>();

            foreach (var file in embeddedFiles)
            {
                var segments = file.Split(splitSeparator);
                var fileName = segments[^2];
                var extension = segments[^1];

                using var contentStream = _assembly.GetManifestResourceStream(file);

                if (contentStream is null) return resourceList;

                using var reader = new StreamReader(contentStream);
                var result = reader.ReadToEnd();

                resourceList.Add(
                    UiResource.Create($"{fileName}.{extension}", result,
                        ContentType.FromExtension(extension)));
            }

            return resourceList;
        }
    }
}
