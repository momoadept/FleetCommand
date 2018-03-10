using FC.Core.Core.ComponentModel;
using FC.Core.Core.FakeAsync.Promises;
using FC.Core.Core.Interfaces;
using VRageMath;

namespace FC.ShipControls
{
    public interface IShipControlStrategy : IStateStrategy<ShipControlState>
    {
        Promise MoveTo(Vector3D coords);
    }
}