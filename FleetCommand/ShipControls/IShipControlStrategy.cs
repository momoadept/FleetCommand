using IngameScript.Core.ComponentModel;
using IngameScript.Core.FakeAsync.Promises;
using VRageMath;

namespace IngameScript.ShipControls
{
    public interface IShipControlStrategy : IStateStrategy<ShipControlState>
    {
        Promise MoveTo(Vector3D coords);
    }
}