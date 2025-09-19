using DVG.Core;
using DVG.SkyPirates.Server.IServices;

namespace DVG.SkyPirates.Server.Services.CommandMutators
{
    public class EmptyCommandMutator : ICommandMutator<ICommandData>
    {
        public Command<ICommandData> Mutate(Command<ICommandData> cmd) => cmd;
    }
}
