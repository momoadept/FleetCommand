using System.Text;
using FC.Core.Core.ComponentModel;

namespace FC.Core.Core.Messaging
{
    public interface IMessageHub
    {
        void SubscribeToActionInvokations(IActionProvider actionProvider);
    }
}
