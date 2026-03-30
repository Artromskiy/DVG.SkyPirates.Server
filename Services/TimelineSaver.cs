using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;
using System.IO;

namespace DVG.SkyPirates.Server.Services
{
    public class TimelineSaver : ITickableExecutor
    {
        private readonly TimelineWriter _writer;
        private readonly ICommandExecutorService _commands;

        public TimelineSaver(TimelineWriter writer, ICommandExecutorService commands)
        {
            _writer = writer;
            _commands = commands;
        }

        public void Tick(int tick)
        {
            if (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.H)
                return;

            Save();
        }

        public void Save()
        {
            var path = GetPath("Snapshots2");
            Save(path, GetObj());
        }

        private object GetObj()
        {
            var snaphsots = _writer.GetSnapshots();
            var commands = _commands.GetCommands();
            return (snaphsots, commands);
        }

        private static void Save(string path, object obj)
        {
            ArrayBufferWriter<byte> buffer = new();
            SerializationUTF8.SerializeCompressed(obj, buffer);
            using var file = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
            file.Write(buffer.WrittenSpan);
        }

        private static string GetPath(string fileName)
        {
            var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            const string folder = "TimelineDebug";
            var path = Path.Combine(basePath, folder, fileName);
            path = Path.ChangeExtension(path, "txt");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }
    }
}
