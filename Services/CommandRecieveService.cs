using DVG.Collections;
using DVG.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Services;
using Riptide;
using System;
using System.Diagnostics;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandRecieveService : ICommandRecieveService
    {
        private readonly Riptide.Server _server;
        private readonly ICheatLoggerService _cheatLogger;
        private readonly ICommandValidatorService _commandValidator;
        private readonly ICommandMutatorService _commandMutator;

        private readonly MessageIO _messageIO;
        private readonly GenericCollection _listeners = new();

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
            _server.MessageReceived += OnMessageRecieved;
        }

        private void OnMessageRecieved(object? _, MessageReceivedEventArgs e)
        {
            var caller = new Caller(e.FromConnection.Id, e.Message, _messageIO, this);
            CommandsRegistry.Call(e.MessageId, ref caller);
        }

        private void InvokeCommand<T>(Command<T> cmd, int clientId)
        {
            if (cmd.ClientId != clientId)
                throw new InvalidOperationException();

            cmd = cmd.WithClientId(clientId);
            if (!_commandValidator.ValidateCommand(cmd))
                throw new InvalidOperationException();

            InvokeCommand(cmd);
        }

        public void InvokeCommand<T>(Command<T> cmd)
        {
            cmd = _commandMutator.Mutate(cmd);
            if (_listeners.TryGet<Action<Command<T>>>(out var callback))
                callback.Invoke(cmd);
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
            private readonly CommandRecieveService _recieveService;

            public Caller(int clientId, Message message, MessageIO messageIO, CommandRecieveService recieveService)
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
                Debug.WriteLine(typeof(T).Name);
                _recieveService.InvokeCommand(command, _clientId);
            }
        }
    }
}