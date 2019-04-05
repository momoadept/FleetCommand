using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        // Send: R|1|messageId|ctrl.action|myTag|{argument}
        // Return: R|2|messageId|{result}
        public class RpcMessage
        {
            public RpcMessageType Type;
            public string Id, ControllerName, ActionName, ReturnTag, Data;

            public override string ToString()
            {
                switch (Type)
                {
                    case RpcMessageType.ActionRequest:
                        return $"R|1|{Id}|{ControllerName}.{ActionName}|{ReturnTag}|{Data}";
                    default:
                        return $"R|2|{Id}|{Data}";
                }
            }

            public RpcMessage()
            {
            }

            public RpcMessage(string message)
            {
                var messageParts = message.Split('|');
                Type = (RpcMessageType) int.Parse(messageParts[1]);
                Id = messageParts[2];
                if (Type == RpcMessageType.ActionRequest)
                    ParseActionRequest(messageParts);
                else
                    ParseReturnedResult(messageParts);
            }

            private void ParseActionRequest(string[] messageParts)
            {
                ReturnTag = messageParts[4];
                Data = string.Join("", messageParts.Skip(5));

                var path = messageParts[3].Split('.');
                ControllerName = path[0];
                ActionName = path[1];
            }

            private void ParseReturnedResult(string[] messageParts)
            {
                Data = string.Join("", messageParts.Skip(3));
            }
        }
    }
}
