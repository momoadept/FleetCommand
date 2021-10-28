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
        public class InvManagerConfig
        {
            public Tag Tag = new Tag("I", true);
            public bool IgnoreUntagged = true;
            public int StepsPerTick = 1;
            public int TransfersPerStep = 100;
            public Tag SummaryLcdTag = new Tag("IOUT");
            public int SummaryTypeWidth = 20;

            public InvManagerConfig()
            {
                
            }

            public InvManagerConfig(string customData)
            {
                var sectionName = "InventoryManager";

                var ok = Aos.Ini.TryParse(customData);

                if (!ok)
                    return;
                
                Aos.Ini.Get(sectionName, "IgnoreUntagged").TryGetBoolean(out IgnoreUntagged);
                Aos.Ini.Get(sectionName, "StepsPerTick").TryGetInt32(out StepsPerTick);
                Aos.Ini.Get(sectionName, "TransfersPerStep").TryGetInt32(out TransfersPerStep);

                var tag = Aos.Ini.Get(sectionName, "Tag");
                if (!tag.IsEmpty)
                    Tag = new Tag(tag.ToString(), true);

                tag = Aos.Ini.Get(sectionName, "SummaryLcdTag");
                if (!tag.IsEmpty)
                    SummaryLcdTag = new Tag(tag.ToString(), true);
            }
        }
    }
}