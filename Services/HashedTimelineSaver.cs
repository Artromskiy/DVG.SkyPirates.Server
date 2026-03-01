using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;
using System.IO;

namespace DVG.SkyPirates.Server.Services
{
    public class HashedTimelineSaver : ITickableExecutor
    {
        private readonly IHashSumService _hashSumService;

        public HashedTimelineSaver(IHashSumService hashSumService)
        {
            _hashSumService = hashSumService;
        }

        public void Tick(int tick)
        {
            if (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.H)
                return;

            const string folder = "HashSum";
            const string fileName = "Hashes";

            var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var path = Path.Combine(basePath, folder, fileName);
            path = Path.ChangeExtension(path, "data");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            ArrayBufferWriter<byte> buffer = new();
            SerializationUTF8.SerializeCompressed(buffer, _hashSumService.GetSnapshots());

            using var file = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
            file.Write(buffer.WrittenSpan);
        }
    }
}
