using DVG.Core;
using Newtonsoft.Json;
using System.IO;

namespace DVG.SkyPirates.Server.Factories
{
    internal class ResourcesFactory<T> : IPathFactory<T>
    {
        public T Create(string parameters)
        {
            var text = File.ReadAllText(parameters + ".json");
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
