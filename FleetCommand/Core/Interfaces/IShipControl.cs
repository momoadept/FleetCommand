using IngameScript.Core.ComponentModel;
using IngameScript.Core.FakeAsync.Promises;
using IngameScript.ShipControls;
using VRageMath;

namespace IngameScript.Core.Interfaces
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
