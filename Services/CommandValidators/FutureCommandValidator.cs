using DVG.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Server.Services.CommandValidators
{
    public class FutureCommandValidator : IGeneralCommandValidator
    {
        private readonly ITickCounterService _tickCounter;

        public FutureCommandValidator(ITickCounterService tickCounter)
        {
            _tickCounter = tickCounter;
        }

        public bool IsValid<T>(Command<T> cmd)
        {
            return _tickCounter.TickCounter >= cmd.Tick;
        }
    }
}
