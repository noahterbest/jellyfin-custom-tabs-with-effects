using System.Reflection;

namespace Jellyfin.Plugin.CustomTabs.Helpers
{
    public static class EmbeddedResourceReader
    {
        private static readonly string Namespace = typeof(CustomTabsPlugin).Namespace!;

        public static string Read(string relativePath)
        {
            Stream? stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"{Namespace}.{relativePath}");

            if (stream == null)
                throw new FileNotFoundException($"Embedded resource not found: {Namespace}.{relativePath}");

            using TextReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
