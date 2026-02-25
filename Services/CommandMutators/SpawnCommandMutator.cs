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
        private readonly IEntityRegistry _entityRegistry;

        public SpawnCommandMutator(IEntityRegistry entityRegistry)
        {
            _entityRegistry = entityRegistry;
        }

        public Command<SpawnUnitCommand> Mutate(Command<SpawnUnitCommand> cmd)
        {
            var syncId = _entityRegistry.Reserve();
            var syncIdReserve = _entityRegistry.Reserve(10);
            var randomSeed = _random.Next();
            cmd.Data.CreationData = new(syncId, syncIdReserve, randomSeed);
            return cmd;
        }

        public Command<SpawnSquadCommand> Mutate(Command<SpawnSquadCommand> cmd)
        {
            var syncId = _entityRegistry.Reserve();
            var syncIdReserve = _entityRegistry.Reserve(10);
            var randomSeed = _random.Next();
            cmd.Data.CreationData = new(syncId, syncIdReserve, randomSeed);
            return cmd;
        }
    }
}