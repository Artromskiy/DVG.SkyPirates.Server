using DVG.Commands;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandValidator { }

    public interface IConcreteCommandValidator<T> : ICommandValidator
    {
        bool IsValid(Command<T> cmd);
    }

    public interface IGeneralCommandValidator : ICommandValidator
    {
        bool IsValid<T>(Command<T> cmd);
    }
}