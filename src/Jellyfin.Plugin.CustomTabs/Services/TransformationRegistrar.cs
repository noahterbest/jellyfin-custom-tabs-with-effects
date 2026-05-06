using System.Reflection;
using System.Runtime.Loader;
using Jellyfin.Plugin.CustomTabs.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.CustomTabs.Services
{
    public static class TransformationRegistrar
    {
        public static bool Register(ILogger<CustomTabsPlugin> logger, IEnumerable<TransformationRegistration> registrations)
        {
            Assembly? assembly = AssemblyLoadContext.All
                .SelectMany(x => x.Assemblies)
                .FirstOrDefault(x => x.FullName?.Contains(".FileTransformation") ?? false);

            if (assembly == null)
            {
                logger.LogWarning("FileTransformation assembly not found. Skipping registration.");
                return false;
            }

            Type? interfaceType = assembly.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface");

            if (interfaceType == null)
            {
                logger.LogWarning("PluginInterface type not found in FileTransformation assembly.");
                return false;
            }

            MethodInfo? method = interfaceType.GetMethod("RegisterTransformation");

            if (method == null)
            {
                logger.LogWarning("RegisterTransformation method not found on PluginInterface.");
                return false;
            }

            foreach (TransformationRegistration reg in registrations)
            {
                method.Invoke(null, new object?[] { reg.ToJObject() });
                logger.LogInformation($"Registered transformation: {reg.FileNamePattern}");
            }

            return true;
        }
    }
}
