using System.Text.RegularExpressions;
using Jellyfin.Plugin.CustomTabs.Helpers;
using Jellyfin.Plugin.CustomTabs.Model;

namespace Jellyfin.Plugin.CustomTabs.Transformers
{
    public static class IndexHtmlTransformer
    {
        public static string FileNamePattern => "index.html";

        public static string Transform(PatchRequestPayload payload)
        {
            string scriptContent = EmbeddedResourceReader.Read("Inject.addCustomTabs.js");
            return Regex.Replace(payload.Contents!, "(</body>)", $"<script defer>{scriptContent}</script>$1");
        }
    }
}
