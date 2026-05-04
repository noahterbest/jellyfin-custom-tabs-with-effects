using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.CustomTabs.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public TabConfig[] Tabs { get; set; } = Array.Empty<TabConfig>();
    }

    public class TabConfig
    {
        public string ContentHtml { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public bool GlowEnabled { get; set; } = false;

        public string GlowColor { get; set; } = "#00ffff";

        public int GlowIntensity { get; set; } = 10;

        public bool HeartbeatEnabled { get; set; } = false;

        public int HeartbeatSpeed { get; set; } = 800;

        public bool RgbAnimationEnabled { get; set; } = false;

        public int RgbSpeed { get; set; } = 2000;
    }
}