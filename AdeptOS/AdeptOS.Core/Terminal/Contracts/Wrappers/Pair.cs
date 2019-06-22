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
        public class Pair<T1, T2>: BaseDataObject<Pair<T1, T2>>
            where T1: IStringifiable, new()
            where T2: IStringifiable, new()
        {
            public T1 First;
            public T2 Second;

            static List<Property<Pair<T1, T2>>> _mapping = new List<Property<Pair<T1, T2>>>()
            {
                new Property<Pair<T1, T2>>("1", it => it.First, (it, first) => (it.First = new T1()).Restore(first)),
                new Property<Pair<T1, T2>>("2", it => it.Second, (it, second) => (it.Second = new T2()).Restore(second))
            };

            public Pair() : base(_mapping)
            {
                
            }

            public Pair(T1 first, T2 second) : base(_mapping)
            {
                First = first;
                Second = second;
            }
        }
    }
}
