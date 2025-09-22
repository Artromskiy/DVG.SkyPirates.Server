using DVG.Core;
using DVG.Core.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.IServices;
using Riptide;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandRecieveService : ICommandRecieveService
    {
        private readonly Riptide.Server _server;
        private readonly ICheatLoggerService _cheatLogger;
        private readonly ICommandSerializer _commandSerializer;
        private readonly ICommandValidatorService _commandValidator;
        private readonly ICommandMutatorService _commandMutator;

        private readonly Dictionary<int, IActionContainer> _registeredRecievers;

        public CommandRecieveService(
            Riptide.Server server,
            ICommandSerializer commandSerializer,
            ICheatLoggerService cheatLogger,
            ICommandValidatorService commandValidator,
            ICommandMutatorService commandMutator)
        {
            _server = server;
            _commandSerializer = commandSerializer;
            _cheatLogger = cheatLogger;
            _commandValidator = commandValidator;
            _commandMutator = commandMutator;
            _registeredRecievers = new Dictionary<int, IActionContainer>();

            _server.MessageReceived += OnMessageRecieved;
        }

        private void OnMessageRecieved(object? _, MessageReceivedEventArgs e)
        {
            if (_registeredRecievers.TryGetValue(e.MessageId, out var callback))
                callback.Invoke(e.Message, e.FromConnection.Id);
        }

        public void RegisterReciever<T>(Action<Command<T>> reciever)
            where T : ICommandData
        {
            int id = CommandIds.GetId<T>();
            ActionContainer<T> genericContainer;
            if (!_registeredRecievers.TryGetValue(id, out var container))
                _registeredRecievers.Add(id, genericContainer = CreateActionContainer());
            else
                genericContainer = (ActionContainer<T>)container;

            genericContainer.Recievers += reciever;

            ActionContainer<T> CreateActionContainer()
            {
                return new ActionContainer<T>(_commandSerializer, _cheatLogger, _commandValidator, _commandMutator);
            }
        }

        public void UnregisterReciever<T>(Action<Command<T>> reciever)
            where T : ICommandData
        {
            int id = CommandIds.GetId<T>();
            if (!_registeredRecievers.TryGetValue(id, out var container))
                return;

            ActionContainer<T> genericContainer = (ActionContainer<T>)container;
            genericContainer.Recievers -= reciever;
            if (!genericContainer.HasTargets)
                _registeredRecievers.Remove(id);
        }

        public void InvokeCommand<T>(Command<T> cmd) where T : ICommandData
        {
            if (_registeredRecievers.TryGetValue(cmd.CommandId, out var callback) &&
                callback is ActionContainer<T> castedCallback)
                castedCallback.Invoke(cmd);
        }

        private class ActionContainer<T> : IActionContainer
            where T : ICommandData
        {
            public event Action<Command<T>>? Recievers;
            public bool HasTargets => Recievers?.GetInvocationList().Length > 0;

            private byte[] _tempBytes = Array.Empty<byte>();

            private readonly ICommandSerializer _commandSerializer;
            private readonly ICheatLoggerService _cheatLogger;
            private readonly ICommandValidatorService _commandsValidator;
            private readonly ICommandMutatorService _commandMutator;

            public ActionContainer(
                ICommandSerializer commandSerializer,
                ICheatLoggerService cheatLogger,
                ICommandValidatorService commandsValidator,
                ICommandMutatorService commandMutator)
            {
                _commandSerializer = commandSerializer;
                _cheatLogger = cheatLogger;
                _commandsValidator = commandsValidator;
                _commandMutator = commandMutator;
            }

            public void Invoke(Message m, int clientId)
            {
                Command<T> cmd = GetCommand(m);
                //if (_cheatLogger.AssertCheating(clientId != cmd.ClientId, clientId, CheatingId.Constants.WrongClientId))
                //    return;

                cmd = cmd.WithClientId(clientId);

                if (!_commandsValidator.ValidateCommand(cmd))
                    return;

                Invoke(cmd);
            }

            public void Invoke(Command<T> cmd)
            {
                cmd = _commandMutator.Mutate(cmd);
                Recievers?.Invoke(cmd);
            }

            private Command<T> GetCommand(Message message)
            {
                var length = (int)message.GetVarULong();
                if (_tempBytes.Length < length)
                    Array.Resize(ref _tempBytes, length);
                message.GetBytes(length, _tempBytes);
                return _commandSerializer.Deserialize<T>(_tempBytes.AsMemory(0, length));
            }
        }

        private interface IActionContainer
        {
            void Invoke(Message message, int clientId);
        }
    }
}