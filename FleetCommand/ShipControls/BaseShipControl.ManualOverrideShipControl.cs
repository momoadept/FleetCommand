using FC.Core.Core.FakeAsync.Promises;
using VRageMath;

namespace IngameScript.ShipControls
{
    public partial class BaseShipControl
    {
        protected class ManualOverrideShipControl : IShipControlStrategy
        {
            public ManualOverrideShipControl(BaseShipControl @base)
            {
                Base = @base;
            }
            protected BaseShipControl Base;

            public void ActivateState(ShipControlState previous, ShipControlState next)
            {
                Base.ResetAutopilot();
            }

            public void DeactivateState(ShipControlState previous, ShipControlState next)
            {
            }

            public Promise MoveTo(Vector3D coords)
            {
                Base.Log.Warning("MoveTo execution ignored: ship on manual override");
                return Promise.Resolve(() => {});
            }
        }
    }
}