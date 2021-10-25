using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public class LcdTracer : IModule, ILcdTracer
        {
            public Dictionary<string, IActionContract> Actions { get; } = new Dictionary<string, IActionContract>();
            public string UniqueName => "LcdTracer";
            public string Alias => "Lcd Tracer";

            int _cacheSize;

            Dictionary<string, IList<IMyTextPanel>> _lcdsByUnwrappedTag =
                new Dictionary<string, IList<IMyTextPanel>>();

            Dictionary<string, Func<string>> _traceSourcesByUnwrappedTag =
                new Dictionary<string, Func<string>>();

            Dictionary<string, Queue<string>> _traceCacheByUnwrappedTag =
                new Dictionary<string, Queue<string>>();

            IGameContext _context;
            ILog _log;

            List<IMyTerminalBlock> bbuffer = new List<IMyTerminalBlock>();

            public LcdTracer(int cacheSize = 10)
            {
                _cacheSize = cacheSize;
                Actions.Add( "LogLastTraces", new ActionContract<Primitive<string>, Void>("LogLastTraces", primitive => LogLastTraces(new Tag(primitive.Value))));
            }

            public void Bind(IBindingContext context)
            {
                _context = context.RequireOne<IGameContext>();
                _log = context.RequireOne<ILog>();
            }

            public void Run()
            {
                var isDev = Aos.Seettings.IsDev;

                Aos.Async.CreateJob(DetectBlocks, Priority.Unimportant).Start();
                Aos.Async.CreateJob(Trace, isDev ? Priority.Critical : Priority.Routine).Start();
                Aos.Async.CreateJob(ClearCache, Priority.Unimportant).Start();

                _log.Info("Lcd Tracer started");
            }

            public void OnSaving()
            {
            }

            public void SetTrace(Tag tag, Func<string> tracer, Priority updatePriority = Priority.Routine)
            {
                if (Aos.Seettings.IsDev)
                    updatePriority = Priority.Critical;

                if (_traceSourcesByUnwrappedTag.ContainsKey(tag.Unwrapped))
                    RemoveTrace(tag);

                _traceSourcesByUnwrappedTag.Add(tag.Unwrapped, tracer);
                _traceCacheByUnwrappedTag.Add(tag.Unwrapped, new Queue<string>());
                DetectBlocks();
            }

            public void RemoveTrace(Tag tag)
            {
                if (_traceCacheByUnwrappedTag.ContainsKey(tag.Unwrapped))
                    _traceCacheByUnwrappedTag.Remove(tag.Unwrapped);

                if (_traceSourcesByUnwrappedTag.ContainsKey(tag.Unwrapped))
                    _traceSourcesByUnwrappedTag.Remove(tag.Unwrapped);
            }

            public IPromise<Void> LogLastTraces(Tag tag)
            {
                foreach (var cache in _traceCacheByUnwrappedTag[tag.Unwrapped])
                    _log.Info(cache);

                return Void.Promise();
            }

            void Trace()
            {
                foreach (var traceSource in _traceSourcesByUnwrappedTag)
                {
                    var newTrace = traceSource.Value();
                    _traceCacheByUnwrappedTag[traceSource.Key].Enqueue(newTrace);

                    if (_lcdsByUnwrappedTag.ContainsKey(traceSource.Key))
                        foreach (var lcd in _lcdsByUnwrappedTag[traceSource.Key])
                            lcd.WriteText(newTrace);
                }
            }

            void DetectBlocks()
            {
                _context.Grid.GetBlocksOfType<IMyTextPanel>(bbuffer);
                _lcdsByUnwrappedTag.Clear();
                foreach (var block in bbuffer)
                {
                    
                    var tags = _traceSourcesByUnwrappedTag.Keys.Where(k => block.CustomName.Contains(k)).Select(x => new Tag(x));
                    var tag = tags.FirstOrDefault(x => _traceSourcesByUnwrappedTag.Keys.Any( k => x.Unwrapped.Equals(k)));
                    
                    if (tag != null)
                    {
                        if (!_lcdsByUnwrappedTag.ContainsKey(tag.Unwrapped))
                            _lcdsByUnwrappedTag.Add(tag.Unwrapped, new List<IMyTextPanel>());

                        var panel = block as IMyTextPanel;
                        panel.ContentType = ContentType.TEXT_AND_IMAGE;
                        //panel.WriteText("Trace LCD " + tag.Wrapped);
                        _lcdsByUnwrappedTag[tag.Unwrapped].Add(panel);
                    }
                }
            }

            void ClearCache()
            {
                foreach (var cache in _traceCacheByUnwrappedTag.Values)
                {
                    while (cache.Count > _cacheSize)
                        cache.Dequeue();
                }
            }
        }
    }
}