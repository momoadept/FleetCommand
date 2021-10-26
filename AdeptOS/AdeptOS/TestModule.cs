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
        public class TestModule : IModule
        {
            public string UniqueName { get; }
            public string Alias { get; }

            ILog _log;

            public void Bind(IBindingContext context)
            {
                _log = context.RequireOne<ILog>();
            }

            public void Run()
            {
                var testName = "[TAGS][TAG:12345] nameTAGTAGS";
                var testName2 = "[TAGS] nameTAGTAGS";
                _log.Info("name: ", testName);

                var testTag1 = new Tag("TAG", true);

                var foundTags = Tag.FromName(testName);
                _log.Info("Found tags in name: ");
                _log.Info(foundTags.Select(x => x.Stringify()).ToArray());

                _log.Info("Contains TAG: ", testTag1.NameMatches(testName).ToString());
                _log.Info("Wrong name Contains TAG: ", testTag1.NameMatches(testName2).ToString());
                var opt = testTag1.GetOptions(testName);
                _log.Info("Tag options: ", opt);

                _log.Info("Contains TAGS: ", new Tag("TAGS").NameMatches(testName).ToString());
            }

            public void OnSaving()
            {
            }
        }
    }
}