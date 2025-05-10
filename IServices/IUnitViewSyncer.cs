using DVG.SkyPirates.Shared.IViews;

namespace DVG.SkyPirates.Server.IServices
{
    internal interface IUnitViewSyncer
    {
        void RegisterView(IUnitView view);
    }
}
