using DVG.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using System;

namespace DVG.SkyPirates.Server.Services.CommandMutators
{
    public class SpawnCommandMutator :
        ICommandMutator<SpawnSquadCommand>,
        ICommandMutator<SpawnUnitCommand>
    {
        private readonly Random _random = new();
        private readonly IEntityRegistryService _entityRegistryService;

        public SpawnCommandMutator(IEntityRegistryService entityRegistryService)
        {
            _entityRegistryService = entityRegistryService;
        }

        public Command<SpawnUnitCommand> Mutate(Command<SpawnUnitCommand> cmd)
        {
            var syncId = _entityRegistryService.Reserve();
            var syncIdReserve = _entityRegistryService.Reserve(10);
            var randomSeed = _random.Next();
            cmd.Data.CreationData = new(syncId, syncIdReserve, randomSeed);
            return cmd;
        }

        public Command<SpawnSquadCommand> Mutate(Command<SpawnSquadCommand> cmd)
        {
            var syncId = _entityRegistryService.Reserve();
            var syncIdReserve = _entityRegistryService.Reserve(10);
            var randomSeed = _random.Next();
            cmd.Data.CreationData = new(syncId, syncIdReserve, randomSeed);
            return cmd;
        }
    }
}