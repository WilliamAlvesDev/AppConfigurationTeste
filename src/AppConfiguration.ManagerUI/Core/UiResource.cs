using System;

namespace AppConfiguration.ManagerUI.Core
{
    internal class UiResource
    {
        public string Content { get; internal set; }

        public string ContentType { get; }

        public string FileName { get; }

        private UiResource(string fileName, string content, string contentType)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));

            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));

            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        public static UiResource Create(string fileName, string content, string contentType) =>
            new UiResource(fileName, content, contentType);
    }
}
