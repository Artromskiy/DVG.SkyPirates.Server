using DVG.Core;
using DVG.SkyPirates.Server.IFactories;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Models;
using System.Collections.Generic;

namespace DVG.SkyPirates.Server.Presenters
{
    internal class WorldPresenter : ITickable
    {
        private readonly IInputFactory _inputFactory;
        private readonly IUnitFactory _unitFactory;
        private readonly IPathFactory<SquadModel> _squadModelFactory;
        private readonly IPathFactory<PackedCirclesModel> _packedCirclesFactory;
        private readonly IClientConnectionService _clientConnectionService;

        private readonly Dictionary<ulong, PlayerPm> _players = new();

        public WorldPresenter(IInputFactory inputFactory, IUnitFactory unitFactory,
            IPathFactory<SquadModel> squadModelFactory,
            IPathFactory<PackedCirclesModel> packedCirclesFactory,
            IClientConnectionService clientConnectionService)
        {
            _inputFactory = inputFactory;
            _unitFactory = unitFactory;
            _squadModelFactory = squadModelFactory;
            _packedCirclesFactory = packedCirclesFactory;
            _clientConnectionService = clientConnectionService;

            _clientConnectionService.OnClientConnected += OnClientConnected;
        }

        public void Tick()
        {
            foreach (var item in _players)
                item.Value.Tick();
        }

        private void OnClientConnected(ushort clientId)
        {
            var input = _inputFactory.Create(clientId);

            var squad = new SquadPm(input, _packedCirclesFactory);
            var squadModel = _squadModelFactory.Create("Configs/SquadModels/DefaultSquadModel");
            var player = new PlayerPm(input, squad, squadModel, _unitFactory);
            _players.Add(clientId, player);
        }
    }
}