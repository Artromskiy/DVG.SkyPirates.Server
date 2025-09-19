using DVG.Core;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandValidator { }

    public interface IConcreteCommandValidator<T> : ICommandValidator
        where T : ICommandData
    {
        bool Validate(Command<T> cmd);
    }

    public interface IGeneralCommandValidator : ICommandValidator
    {
        bool Validate<T>(Command<T> cmd) where T : ICommandData;
    }
}