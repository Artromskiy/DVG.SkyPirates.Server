using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Entities;

namespace DVG.SkyPirates.Server
{
    public class WorldIniter
    {
        private readonly World _world;
        private readonly IPathFactory<HexMap> _hexMapLoader;

        public WorldIniter(World world, IPathFactory<HexMap> hexMapLoader)
        {
            _world = world;
            _hexMapLoader = hexMapLoader;
        }

        public void Init()
        {
            var hexMap = _hexMapLoader.Create("Configs/Maps/Map");
            _world.AddOrGet<HexMap>(EntityIds.Get(1)) = hexMap;
        }
    }
}
