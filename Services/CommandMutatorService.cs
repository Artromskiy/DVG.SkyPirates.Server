using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Server.Services
{
    public class CommandMutatorService : ICommandMutatorService
    {
        private readonly ICommandMutator[] _mutators;

        public CommandMutatorService(IEnumerable<ICommandMutator> mutators)
        {
            _mutators = mutators.ToArray();
        }

        public Command<T> Mutate<T>(Command<T> cmd) where T : ICommandData
        {
            foreach (var item in _mutators)
                if (item is ICommandMutator<T> concrete)
                    cmd = concrete.Mutate(cmd);

            return cmd;
        }
    }
}
