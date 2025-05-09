using DVG.SkyPirates.Server.Services;
using DVG.SkyPirates.Server.Views;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IViews;

namespace DVG.SkyPirates.Server.Factories
{
    class NetworkedUnitViewFactory : IUnitViewFactory
    {
        private readonly IUnitViewSyncer _syncer;

        public NetworkedUnitViewFactory(IUnitViewSyncer syncer)
        {
            _syncer = syncer;
        }

        public IUnitView Create((UnitId unitId, int level, int merge) parameters)
        {
            var view = new UnitView();
            _syncer.RegisterView(view);
            return view;
        }
    }
}
