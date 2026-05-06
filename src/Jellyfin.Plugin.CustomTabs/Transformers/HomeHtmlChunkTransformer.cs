using System.Text.RegularExpressions;
using Jellyfin.Plugin.CustomTabs.Configuration;
using Jellyfin.Plugin.CustomTabs.Helpers;
using Jellyfin.Plugin.CustomTabs.Model;

namespace Jellyfin.Plugin.CustomTabs.Transformers
{
    public static class HomeHtmlChunkTransformer
    {
        public static string FileNamePattern => "home-html\\..*\\.chunk\\.js";

        public static string Transform(PatchRequestPayload payload)
        {
            string tabTemplate = EmbeddedResourceReader.Read("Inject.tabTemplate.html");
            TabConfig[] tabs = CustomTabsPlugin.Instance.Configuration.Tabs;

            string finalReplacement = string.Concat(
                tabs.Select((tab, i) => tabTemplate
                    .Replace("{{tab_id}}", $"customTab_{i}")
                    .Replace("{{tab_index}}", $"{i + 2}")
                    .Replace("{{tab_content}}", tab.ContentHtml))
            );

            finalReplacement = finalReplacement
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("  ", " ")
                .Replace("'undefined'", "\\'undefined\\'");

            return Regex.Replace(
                payload.Contents!,
                "(id=\"favoritesTab\" data-index=\"1\"> <div class=\"sections\"></div> </div>)",
                $"$1{finalReplacement}");
        }
    }
}
