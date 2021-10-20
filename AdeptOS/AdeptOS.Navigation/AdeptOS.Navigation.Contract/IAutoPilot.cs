using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public interface ICoordinates
        {
            float X { get; set; }
            float Y { get; set; }
            float Z { get; set; }
        }

        public class Coordinates : Pair<Primitive<float>, Pair<Primitive<float>, Primitive<float>>>, ICoordinates
        {
            public float X
            {
                get { return First.Value; }
                set { First.Value = value; }
            }

            public float Y
            {
                get { return Second.First.Value; }
                set { Second.First.Value = value; }
            }

            public float Z
            {
                get { return Second.Second.Value; }
                set { Second.Second.Value = value; }
            }

            public Coordinates()
            {
                First = 0f.AsPrimitive();
                Second = new Pair<Primitive<float>, Primitive<float>>(0f.AsPrimitive(), 0f.AsPrimitive());
            }
        }

        public interface IAutoPilot
        {
            IPromise<Void> NavigateToCoordinates(Coordinates target);

            IPromise<Void> NavigateToCoordinatesPrecisely(Coordinates target, Coordinates orientation);

        }
    }
}
