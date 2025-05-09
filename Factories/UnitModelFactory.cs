using DVG.Core;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Models;

namespace DVG.SkyPirates.Server.Factories
{
    internal class UnitModelFactory : IUnitModelFactory
    {
        private readonly IPathFactory<UnitModel> _pathFactory;

        public UnitModelFactory(IPathFactory<UnitModel> pathFactory)
        {
            _pathFactory = pathFactory;
        }

        public UnitModel Create((UnitId unitId, int level, int merge) parameters)
        {
            return null; //_pathFactory.Create("Configs/Units/" + parameters.unitId.Value);
        }
    }
}
