using DVG.Core;
using DVG.Core.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Services;
using Riptide;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandRecieveService : ICommandRecieveService
    {
        private readonly Riptide.Server _server;
        private readonly ICheatLoggerService _cheatLogger;
        private readonly ICommandValidatorService _commandValidator;
        private readonly ICommandMutatorService _commandMutator;

        private readonly MessageIO _messageIO;
        private readonly Dictionary<int, IActionInvoker> _actionInvokers = new();
        private readonly Dictionary<int, Delegate> _recievers = new();

        public CommandRecieveService(
            Riptide.Server server,
            ICommandSerializer commandSerializer,
            ICheatLoggerService cheatLogger,
            ICommandValidatorService commandValidator,
            ICommandMutatorService commandMutator)
        {
            _server = server;
            _cheatLogger = cheatLogger;
            _commandValidator = commandValidator;
            _commandMutator = commandMutator;
            _messageIO = new MessageIO(commandSerializer);
            var createInvokers = new CreateInvokersAction(this);
            CommandIds.ForEachData(ref createInvokers);
            _server.MessageReceived += OnMessageRecieved;
        }

        private void OnMessageRecieved(object? _, MessageReceivedEventArgs e)
        {
            if (_actionInvokers.TryGetValue(e.MessageId, out var invoker))
                invoker.Invoke(e.Message, e.FromConnection.Id);
        }

        private void InvokeCommand<T>(Command<T> cmd, int clientId)
             where T : ICommandData
        {
            if (cmd.ClientId != clientId)
                throw new InvalidOperationException();

            cmd = cmd.WithClientId(clientId);
            if (!_commandValidator.ValidateCommand(cmd))
                throw new InvalidOperationException();

            InvokeCommand(cmd);
        }

        public void InvokeCommand<T>(Command<T> cmd) where T : ICommandData
        {
            cmd = _commandMutator.Mutate(cmd);
            if (_recievers.TryGetValue(cmd.CommandId, out var deleg) &&
                deleg is Action<Command<T>> callback)
                callback.Invoke(cmd);
        }

        public void RegisterReciever<T>(Action<Command<T>> reciever)
            where T : ICommandData
        {
            int id = CommandIds.GetId<T>();
            if (!_recievers.TryGetValue(id, out var recievers))
                _recievers.Add(id, reciever);
            else
                _recievers[id] = (recievers as Action<Command<T>>) + reciever;
        }

        public void UnregisterReciever<T>(Action<Command<T>> reciever)
            where T : ICommandData
        {
            int id = CommandIds.GetId<T>();
            if (!_recievers.TryGetValue(id, out var recievers))
                return;
            if (recievers is not Action<Command<T>> genericRecievers)
                throw new InvalidCastException();
            genericRecievers -= reciever;
            if (genericRecievers == null)
                _actionInvokers.Remove(id);
            else
                _recievers[id] = genericRecievers;
        }

        private readonly struct CreateInvokersAction : IGenericAction<ICommandData>
        {
            private readonly CommandRecieveService _recieveService;

            public CreateInvokersAction(CommandRecieveService recieveService)
            {
                _recieveService = recieveService;
            }

            public readonly void Invoke<T>() where T : ICommandData
            {
                _recieveService._actionInvokers[CommandIds.GetId<T>()] =
                    new ActionInvoker<T>(_recieveService);
            }
        }

        private class ActionInvoker<T> : IActionInvoker
            where T : ICommandData
        {
            private readonly CommandRecieveService _recieveService;
            private readonly MessageIO _messageIO;

            public ActionInvoker(CommandRecieveService recieveService)
            {
                _recieveService = recieveService;
                _messageIO = _recieveService._messageIO;
            }

            public void Invoke(Message m, ushort clientId)
            {
                if (!_messageIO.RecieveMessage<T>(m, clientId, out var cmd))
                {
                    return;
                }
                _recieveService.InvokeCommand(cmd, clientId);
            }
        }

        private interface IActionInvoker
        {
            void Invoke(Message message, ushort clientId);
        }
    }
}