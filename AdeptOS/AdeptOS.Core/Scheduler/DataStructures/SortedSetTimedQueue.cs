﻿using Sandbox.Game.EntityComponents;
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
        public class SortedSetTimedQueue<TValue>: ITimedQueue<TValue>
        {
            private SortedSet<DateTime> _keys = new SortedSet<DateTime>();
            private Dictionary<DateTime, List<TValue>> _values = new Dictionary<DateTime, List<TValue>>();
            private DateTime _minTime = DateTime.MinValue;

            public void Push(DateTime time, TValue value)
            {
                if (!_keys.Contains(time))
                {
                    _keys.Add(time);

                    if(!_values.ContainsKey(time))
                        _values.Add(time, new List<TValue>(10));
                }

                _values[time].Add(value);
            }

            public bool AnyLessThan(DateTime time)
            {
                return _keys.GetViewBetween(_minTime, time).Any();
            }

            public IEnumerable<TValue> PopLessThan(DateTime time)
            {
                var keys = _keys.GetViewBetween(_minTime, time);
                _keys.ExceptWith(keys);

                var items = keys
                    .SelectMany(it => _values[it]);
                foreach (var key in keys)
                {
                    _values.Remove(key);
                }

                return items;
            }

            public void Clear()
            {
                _keys.Clear();
                _values.Clear();
            }

            public int Count => _values.SelectMany(it => it.Value).Count();
        }
    }
}