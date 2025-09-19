using DVG.Core;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandMutatorService
    {
        public Command<T> Mutate<T>(Command<T> cmd) where T : ICommandData;
    }
}
