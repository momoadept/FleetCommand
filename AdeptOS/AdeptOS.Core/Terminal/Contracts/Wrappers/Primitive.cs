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
        public class Primitive<T>: IStringifiable
        {
            public T Value;

            public string Stringify() => $"{Value}";

            public T Retrieve(string value)
            {
                Restore(value);
                return Value;
            }

            public void Restore(string value)
            {
                var val = value.Replace("{", "").Replace("}", "");

                if (Value is int)
                    Value = (T)(object)int.Parse(val);
                else if (Value is double)
                    Value = (T)(object)double.Parse(val);
                else if (Value is bool)
                    Value = (T)(object)bool.Parse(val);
                else if (Value is string)
                    Value = (T)(object)val;
            }

            public Primitive()
            {
                
            }

            public Primitive(T value)
            {
                Value = value;
            }
        }
    }
}
