using Delta.Netcode;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandMutatorService
    {
        Command<T> Mutate<T>(Command<T> cmd);
    }
}
