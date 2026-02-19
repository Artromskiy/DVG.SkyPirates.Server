using DVG.Commands;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandMutatorService
    {
        Command<T> Mutate<T>(Command<T> cmd) where T : ICommandData;
    }
}
