using FC.Core.Core.ComponentModel;
using FC.Core.Core.FakeAsync.Promises;
using FC.ShipControls;
using VRageMath;

namespace FC.Core.Core.Interfaces
{
    public interface IShipControl: IStatefull<ShipControlState>
    {
        Promise MoveTo(Vector3D coords);
        Promise Stop();
        Promise<Vector3D?> GetCurrentWaypoint();
        Promise SetManualOverride();
        Promise RemoveManualOverride();
    }
}
