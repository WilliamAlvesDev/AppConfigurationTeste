using System;
using System.Collections.Generic;

namespace AppConfiguration.ManagerUI.Core
{
    internal class ContentType
    {
        public static string Javascript = "text/javascript";
        public static string Css = "text/css";
        public static string Html = "text/html";
        public static string Plain = "text/plain";
        public static string Json = "application/json";

        public static Dictionary<string, string> SupportedContent =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "js", Javascript },
                { "html", Html },
                { "css", Css }
            };

        public static string FromExtension(string fileExtension)
            => SupportedContent.TryGetValue(fileExtension, out var result) ? result : Plain;
    }
}
