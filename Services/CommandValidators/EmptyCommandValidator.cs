using DVG.Commands;
using DVG.SkyPirates.Server.IServices;

namespace DVG.SkyPirates.Server.Services.CommandValidators
{
    public class EmptyCommandValidator : IConcreteCommandValidator<ICommandData>
    {
        public bool Validate(Command<ICommandData> cmd) => true;
    }
}
