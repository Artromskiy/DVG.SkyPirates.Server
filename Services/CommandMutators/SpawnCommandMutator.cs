using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Server.Services.CommandMutators
{
    public class SpawnCommandMutator :
        ICommandMutator<SpawnSquadCommand>,
        ICommandMutator<SpawnUnitCommand>
    {
        private int _newEntityId = 1;

        public Command<SpawnUnitCommand> Mutate(Command<SpawnUnitCommand> cmd) => MutateCommand(cmd);
        public Command<SpawnSquadCommand> Mutate(Command<SpawnSquadCommand> cmd) => MutateCommand(cmd);

        private Command<T> MutateCommand<T>(Command<T> cmd)
            where T : ICommandData
        {
            return cmd.WithEntityId(NewEntityId());
        }

        private int NewEntityId() => ++_newEntityId;
    }
}