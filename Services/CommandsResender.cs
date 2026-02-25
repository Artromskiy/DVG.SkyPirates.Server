using DVG.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandsResender
    {
        public CommandsResender(ICommandRecieveService commandRecieveService, ICommandSendService commandSendService)
        {
            var action = new RegisterResendAction(commandRecieveService, commandSendService);
            CommandsRegistry.ForEach(ref action);
        }

        private readonly struct RegisterResendAction : IGenericAction
        {
            private readonly ICommandRecieveService _commandRecieveService;
            private readonly ICommandSendService _commandSendService;

            public RegisterResendAction(ICommandRecieveService commandRecieveService, ICommandSendService commandSendService)
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
                if (CommandInfos.ClientPredicted<T>())
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
