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
        public class RemoteActionContract<TArgument, TResult>: IActionContract<TArgument, TResult> 
            where TResult : class, IStringifiable, new() 
            where TArgument : class, IStringifiable, new()
        {
            public string Name { get; }
            public bool NoArgument { get; }
            string _controllerName;
            Tag _targetTag;
            IRPC _rpc;

            public RemoteActionContract(IRPC rpc, Tag targetTag, string controllerName, string name, bool noArgument = false)
            {
                _controllerName = controllerName;
                Name = name;
                _targetTag = targetTag;
                _rpc = rpc;
                NoArgument = noArgument;
            }

            public IPromise<TResult> Do(TArgument arg)
            {
                return DoStringed(arg.Stringify());
            }

            public IPromise<object> Do(object arg)
            {
                if (arg is TArgument)
                    return Do((TArgument) arg);

                return DoStringed((string) arg);
            }

            IPromise<TResult> DoStringed(string arg)
            {
                return _rpc.Call(new RpcRoute(_targetTag, _controllerName, Name), arg)
                    .Next(resultString =>
                    {
                        var result = new TResult();
                        result.Restore(resultString);
                        return Promise<TResult>.FromValue(result);
                    });
            }
        }
    }
}
