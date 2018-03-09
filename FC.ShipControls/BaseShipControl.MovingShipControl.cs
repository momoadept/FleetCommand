using FC.Core.Core;
using FC.Core.Core.FakeAsync.Promises;
using FC.Core.Core.Logging;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace FC.ShipControls
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
                Base.Log.Info("Autopilot enabled");
            }

            public void DeactivateState(ShipControlState previous, ShipControlState next)
            {
                Base.Autopilot.SetAutoPilotEnabled(false);
                Base.Log.Info("Autopilot disabled");
            }

            public Promise MoveTo(Vector3D coords)
            {
                Base.Autopilot.ClearWaypoints();
                Base.Autopilot.AddWaypoint(coords, "Autopilot");
                Base.LastWaypointCoords = coords;
                return Base.Async.When(() =>  coords.Equals(Base.Autopilot.GetPosition(), 20), 100)
                    .Then(t => Base.State = ShipControlState.Standby);
            }
        }
    }
}