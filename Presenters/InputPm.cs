using DVG.Core;
using System;

namespace DVG.SkyPirates.Server.Presenters
{
    internal class InputPm : Presenter<IView, object>
    {
        public InputPm() : base(null, null) { }
        public float3 Position { get; set; }
        public float Rotation { get; set; }
        public bool Fixation { get; set; }

        public event Action<int>? OnSpawnUnit;
    }
}
