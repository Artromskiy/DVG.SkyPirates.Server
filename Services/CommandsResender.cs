using DVG.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandsResender
    {
        public CommandsResender(ICommandReciever commandRecieveService, ICommandSender commandSendService)
        {
            var action = new RegisterResendAction(commandRecieveService, commandSendService);
            CommandsRegistry.ForEach(ref action);
        }

        private readonly struct RegisterResendAction : IGenericAction
        {
            private readonly ICommandReciever _commandRecieveService;
            private readonly ICommandSender _commandSendService;

            public RegisterResendAction(ICommandReciever commandRecieveService, ICommandSender commandSendService)
            {
                _commandRecieveService = commandRecieveService;
                _commandSendService = commandSendService;
            }

            public readonly void Invoke<T>()
            {
                _commandRecieveService.RegisterReciever<T>(Send);
            }

            private void Send<T>(Command<T> cmd)
            {
                if (CommandsRegistry.IsPredicted<T>())
                {
                    _commandSendService.SendToAll(cmd, cmd.ClientId);
                }
                else
                {
                    _commandSendService.SendToAll(cmd);
                }
            }
        }
    }
}
