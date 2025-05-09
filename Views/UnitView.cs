using DVG.SkyPirates.Shared.IViews;

namespace DVG.SkyPirates.Server.Views
{
    internal class UnitView : IUnitView
    {
        public float Rotation { get; set; }
        public float3 Position { get; set; }
    }
}