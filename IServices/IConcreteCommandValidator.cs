using DVG.Commands;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandValidator { }

    public interface IConcreteCommandValidator<T> : ICommandValidator
    {
        bool Validate(Command<T> cmd);
    }

    public interface IGeneralCommandValidator : ICommandValidator
    {
        bool Validate<T>(Command<T> cmd);
    }
}