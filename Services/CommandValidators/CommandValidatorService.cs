using DVG.Commands;
using DVG.SkyPirates.Server.IServices;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Server.Services.CommandValidators
{
    internal class CommandValidatorService : ICommandValidatorService
    {
        private readonly ICommandValidator[] _validators;

        public CommandValidatorService(IEnumerable<ICommandValidator> validators)
        {
            _validators = validators.ToArray();
        }

        public bool ValidateCommand<T>(Command<T> cmd)
        {
            bool isValid = true;

            foreach (var item in _validators)
            {
                if (item is IConcreteCommandValidator<T> concrete)
                    isValid &= concrete.IsValid(cmd);
                if (item is IGeneralCommandValidator general)
                    isValid &= general.IsValid(cmd);
            }

            return !isValid;
        }
    }
}
