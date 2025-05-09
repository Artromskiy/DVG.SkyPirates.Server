using DVG.SkyPirates.Shared.IViews;

namespace DVG.SkyPirates.Server.Services
{
    internal interface IUnitViewSyncer
    {
        void RegisterView(IUnitView view);
    }
}
