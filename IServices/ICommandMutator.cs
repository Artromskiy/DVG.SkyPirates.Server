using DVG.Core;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandMutator { }
    public interface ICommandMutator<T> : ICommandMutator
         where T : ICommandData
    {
        Command<T> Mutate(Command<T> cmd);
    }
}