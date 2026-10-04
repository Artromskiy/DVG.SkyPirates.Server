using DVG.Collections;
using Delta.Netcode;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Services;
using Riptide;
using System;
using CommandsRegistry = DVG.Commands.CommandsRegistry;
using IGenericAction = DVG.Commands.IGenericAction;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandReciever : ICommandReciever
    {
        private readonly Riptide.Server _server;
        private readonly ICheatLoggerService _cheatLogger;
        private readonly ICommandValidatorService _commandValidator;
        private readonly ICommandMutatorService _commandMutator;
        private readonly ICommandSender _commandSender;

        private readonly MessageIO _messageIO;
        private readonly GenericCollection _listeners = new();

        public CommandReciever(
            Riptide.Server server,
            ICheatLoggerService cheatLogger,
            ICommandValidatorService commandValidator,
            ICommandMutatorService commandMutator,
            ICommandSender commandSender)
        {
            _server = server;
            _cheatLogger = cheatLogger;
            _commandValidator = commandValidator;
            _commandMutator = commandMutator;
            _commandSender = commandSender;
            _messageIO = new MessageIO();
            _server.MessageReceived += OnMessageRecieved;
        }

        private void OnMessageRecieved(object? _, MessageReceivedEventArgs e)
        {
            var caller = new Caller(e.FromConnection.Id, e.Message, _messageIO, this);
            CommandsRegistry.Call(e.MessageId, ref caller);
        }

        private void InvokeCommand<T>(Command<T> command, int clientId)
        {
            if (SkyPiratesCommand.GetClientId(command) != clientId)
            {
                // dirty cheater
            }

            command = SkyPiratesCommand.WithClientId(command, clientId);

            if (!_commandValidator.IsValid(command))
            {
                if (GeneratedCommands.GetRegistration<T>().IsPredicted)
                {
                    var invalidate = new InvalidateCommand()
                    {
                        CommandId = checked((int)GeneratedCommands.GetRegistration<T>().Id),
                    };
                    _commandSender.SendTo<InvalidateCommand>(SkyPiratesCommand.Create(clientId, SkyPiratesCommand.GetTick(command), invalidate), clientId);
                }
                else
                {
                    // do nothing
                }
                return;
            }

            InvokeCommand(command);
        }

        public void InvokeCommand<T>(Command<T> command)
        {
            command = _commandMutator.Mutate(command);
            if (_listeners.TryGet<Action<Command<T>>>(out var callback))
                callback.Invoke(command);
        }

        public void RegisterReciever<T>(Action<Command<T>> reciever)
        {
            if (!_listeners.TryGet<Action<Command<T>>>(out var callback))
                _listeners.Add(reciever);
            else
                _listeners.Add(callback + reciever);
        }

        public void UnregisterReciever<T>(Action<Command<T>> reciever)
        {
            if (!_listeners.TryGet<Action<Command<T>>>(out var recievers))
                return;
            recievers -= reciever;
            if (recievers == null)
                _listeners.Remove<Action<Command<T>>>();
            else
                _listeners.Add(reciever);
        }

        private readonly struct Caller : IGenericAction
        {
            private readonly int _clientId;
            private readonly Message _message;
            private readonly MessageIO _messageIO;
            private readonly CommandReciever _recieveService;

            public Caller(int clientId, Message message, MessageIO messageIO, CommandReciever recieveService)
            {
                _clientId = clientId;
                _message = message;
                _messageIO = messageIO;
                _recieveService = recieveService;
            }

            public void Invoke<T>()
            {
                if (!_messageIO.RecieveMessage<T>(_message, _clientId, out var command))
                    return;
                _recieveService.InvokeCommand(command, _clientId);
            }
        }
    }
}