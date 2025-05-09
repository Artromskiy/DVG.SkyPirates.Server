using DVG.Core;
using DVG.SkyPirates.Shared.IViews;
using DVG.SkyPirates.Shared.Models;

namespace DVG.SkyPirates.Server.Presenters
{
    internal class UnitPm : Presenter<IUnitView, UnitModel>, ITickable
    {
        public float3 TargetPosition { get; set; }

        public float3 Position => View.Position;
        private float _angle;

        public UnitPm(IUnitView view, UnitModel model) : base(view, model) { }

        public void Tick()
        {
            RotateTo(1f / 60);
            MoveRoutine();
        }

        private void MoveRoutine()
        {
            var velocity = float2.ClampLength((TargetPosition - Position).xz, 1) * Model.speed;
        }

        private void RotateTo(float deltaTime)
        {

        }
    }
}
