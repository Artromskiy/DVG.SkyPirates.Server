using DVG.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Server.Services.CommandValidators
{
    internal class CommandValidatorService : ICommandValidatorService
    {
        private readonly ICheatLoggerService _cheatLogger;

        private readonly ICommandValidator[] _validators;

        public CommandValidatorService(
            ICheatLoggerService cheatLogger,
            IEnumerable<ICommandValidator> validators)
        {
            _cheatLogger = cheatLogger;
            _validators = validators.ToArray();
        }

        public bool ValidateCommand<T>(Command<T> cmd)
        {
            return true;
            bool isValid = true;

            isValid &= !_cheatLogger.AssertCheating(FutureTimeCheating(cmd), cmd.ClientId, CheatingId.Constants.FutureCommand);

            foreach (var item in _validators)
            {
                if (item is IConcreteCommandValidator<T> concrete)
                    isValid &= concrete.Validate(cmd);
                if (item is IGeneralCommandValidator general)
                    isValid &= general.Validate(cmd);
            }

            return !isValid;
        }

        private static bool FutureTimeCheating<T>(Command<T> cmd)
        {
            return false;// cmd.Tick < TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
        }

        public bool ValidateClientId<T>(int clientId, Command<T> cmd)
        {
            return !_cheatLogger.AssertCheating(clientId != cmd.ClientId, clientId, new CheatingId());
        }
    }
}
