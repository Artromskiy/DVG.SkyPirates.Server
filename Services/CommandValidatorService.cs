using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Server.Services
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

        public bool ValidateCommand<T>(Command<T> cmd) where T : ICommandData
        {
            return true;
            bool isValid = true;

            isValid &= !_cheatLogger.AssertCheating(OwnershipCheating(cmd), cmd.ClientId, CheatingId.Constants.NoOwnershipCommand);
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

        private bool OwnershipCheating<T>(Command<T> cmd) where T : ICommandData
        {
            return false;
            //bool exist = _instanceIdsService.HasEntity(cmd.EntityId);
            //return exist && !_ownershipService.HasOwnership(cmd.ClientId, cmd.EntityId);
        }

        private static bool FutureTimeCheating<T>(Command<T> cmd) where T : ICommandData
        {
            return false;// cmd.Tick < TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
        }

        public bool ValidateClientId<T>(int clientId, Command<T> cmd) where T : ICommandData
        {
            return !_cheatLogger.AssertCheating(clientId != cmd.ClientId, clientId, new CheatingId());
        }
    }
}
