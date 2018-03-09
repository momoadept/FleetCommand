using IngameScript.Core.FakeAsync.Promises;
using VRageMath;

namespace IngameScript.ShipControls
{
    public partial class BaseShipControl
    {
        protected class StandbyShipControl : IShipControlStrategy
        {
            public StandbyShipControl(BaseShipControl @base)
            {
                Base = @base;
            }
            protected BaseShipControl Base;

            public void ActivateState(ShipControlState previous, ShipControlState next)
            {
                Base.Autopilot.SetAutoPilotEnabled(true);
            }

            public void DeactivateState(ShipControlState previous, ShipControlState next)
            {
                Base.ResetAutopilot();
            }

            public Promise MoveTo(Vector3D coords)
            {
                Base.State = ShipControlState.Moving;
                return Base.MoveTo(coords);
            }
        }
    }
}