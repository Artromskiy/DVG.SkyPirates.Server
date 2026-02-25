using DVG.Commands;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandMutator { }
    public interface ICommandMutator<T> : ICommandMutator
    {
        Command<T> Mutate(Command<T> cmd);
    }
}