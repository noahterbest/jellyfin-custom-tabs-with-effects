using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.CustomTabs.Model
{
    public class TransformationRegistration
    {
        public string Id { get; }
        public string FileNamePattern { get; }
        public string CallbackAssembly { get; }
        public string CallbackClass { get; }
        public string CallbackMethod { get; }

        public TransformationRegistration(string id, string fileNamePattern, Type callbackType)
        {
            Id = id;
            FileNamePattern = fileNamePattern;
            CallbackAssembly = callbackType.Assembly.FullName!;
            CallbackClass = callbackType.FullName!;
            CallbackMethod = "Transform";
        }

        public JObject ToJObject() => new()
        {
            ["id"] = Id,
            ["fileNamePattern"] = FileNamePattern,
            ["callbackAssembly"] = CallbackAssembly,
            ["callbackClass"] = CallbackClass,
            ["callbackMethod"] = CallbackMethod
        };
    }
}
