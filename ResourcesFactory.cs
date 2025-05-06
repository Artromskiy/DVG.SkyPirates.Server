using DVG.Core;
using Newtonsoft.Json;
using System.IO;

namespace DVG.SkyPirates.Server
{
    internal class ResourcesFactory<T> : IPathFactory<T>
    {
        public T Create(string parameters)
        {
            var text = File.ReadAllText(parameters);
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
