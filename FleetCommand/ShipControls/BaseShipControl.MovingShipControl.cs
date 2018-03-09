using System;
using FC.Core.Core.FakeAsync.Promises;
using VRageMath;

namespace IngameScript.ShipControls
{
    public partial class BaseShipControl
    {
        protected class MovingShipControl : IShipControlStrategy
        {
            public MovingShipControl(BaseShipControl @base)
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
                Base.Autopilot.ClearWaypoints();
                Base.Autopilot.AddWaypoint(coords, "Autopilot");
                return Base.Async.When(() =>  coords.Equals(Base.Autopilot.Position, 1), 100);
            }
        }
    }
}