using FC.Core.Core.FakeAsync.Promises;
using FC.Core.Core.Interfaces;
using VRageMath;

namespace FC.ShipControls
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
                Base.Autopilot.ClearWaypoints();
            }

            public void DeactivateState(ShipControlState previous, ShipControlState next)
            {
            }

            public Promise MoveTo(Vector3D coords)
            {
                Base.State = ShipControlState.Moving;
                return Base.MoveTo(coords);
            }
        }
    }
}