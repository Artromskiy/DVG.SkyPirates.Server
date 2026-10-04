using Delta.Netcode;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using CommandsRegistry = DVG.Commands.CommandsRegistry;
using IGenericAction = DVG.IGenericAction;

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
                if (GeneratedCommands.GetRegistration<T>().IsPredicted)
                {
                    _commandSendService.SendToAll(cmd, SkyPiratesCommand.GetClientId(cmd));
                }
                else
                {
                    _commandSendService.SendToAll(cmd);
                }
            }
        }
    }
}
