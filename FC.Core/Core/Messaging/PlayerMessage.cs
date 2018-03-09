using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FC.Core.Core.Messaging
{
    public class PlayerMessage
    {
        public string ComponentId;
        public string Action;
        public string[] Args;
        public override string ToString() => $"{ComponentId}.{Action}.{string.Join(".", Args)}";

        public static bool TryParse(string src, out PlayerMessage result)
        {
            var items = src.Split('.');
            try
            {
                result = new PlayerMessage()
                {
                    ComponentId = items[0],
                    Action = items[1],
                    Args = items.Skip(2).ToArray()
                };
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
