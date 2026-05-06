using Jellyfin.Plugin.CustomTabs.JellyfinVersionSpecific;
using Jellyfin.Plugin.CustomTabs.Model;
using Jellyfin.Plugin.CustomTabs.Transformers;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.CustomTabs.Services
{
    public class StartupService : IScheduledTask
    {
        public string Name => "Custom Tabs Startup";
        public string Key => "Jellyfin.Plugin.CustomTabs.Startup";
        public string Description => "Startup Service for Custom Tabs";
        public string Category => "Startup Services";

        private readonly ILogger<CustomTabsPlugin> _logger;

        public StartupService(ILogger<CustomTabsPlugin> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CustomTabs Startup. Registering file transformations.");

            IEnumerable<TransformationRegistration> registrations = new[]
            {
                new TransformationRegistration(
                    "dcaafb64-88de-4efa-b77b-ae0616291cbb",
                    IndexHtmlTransformer.FileNamePattern,
                    typeof(IndexHtmlTransformer)),
                new TransformationRegistration(
                    "403e6374-7433-4137-b24f-2be01a14a90f",
                    HomeHtmlChunkTransformer.FileNamePattern,
                    typeof(HomeHtmlChunkTransformer))
            };

            TransformationRegistrar.Register(_logger, registrations);
            return Task.CompletedTask;
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers() => StartupServiceHelper.GetDefaultTriggers();
    }
}
