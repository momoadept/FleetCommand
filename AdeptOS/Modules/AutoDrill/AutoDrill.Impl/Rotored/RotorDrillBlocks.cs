﻿using Sandbox.Game.EntityComponents;
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
        public class RotorDrillBlocks
        {
            public List<IMyPistonBase> HorizontalPistonArm = new List<IMyPistonBase>();
            public List<IMyPistonBase> VerticalPistonArm = new List<IMyPistonBase>();
            public List<IMyShipDrill> Drill = new List<IMyShipDrill>();
            public IMyTextPanel ReportLcd;
            public IMyMotorStator Rotor;
            public bool Valid => HorizontalPistonArm.Any() && VerticalPistonArm.Any() && Drill.Any() && Rotor != null;
            IMyGridTerminalSystem _grid;
            List<IMyTerminalBlock> _bbuffer;
            List<IMyBlockGroup> _gbuffer;

            public RotorDrillBlocks(IMyGridTerminalSystem grid, List<IMyTerminalBlock> bbuffer = null, List<IMyBlockGroup> gbuffer = null)
            {
                _grid = grid;
                _bbuffer = bbuffer;
                _gbuffer = gbuffer;
            }

            public void SetDrills(bool enabled)
            {
                    foreach (var drill in Drill)
                        drill.Enabled = enabled;
            }

            public bool Refresh()

            {
                _bbuffer = _bbuffer ?? new List<IMyTerminalBlock>();
                _gbuffer = _gbuffer ?? new List<IMyBlockGroup>();

                var verticalTag = new Tag("RD_V");
                var horizontalTag = new Tag("RD_H");
                var drillTag = new Tag("RD_D");
                var rotorTag = new Tag("RD_R");
                var lcdTag = new Tag("RD_S");

                HorizontalPistonArm = Tag.FindGroupByTag<IMyPistonBase>(horizontalTag, _grid, _bbuffer, _gbuffer);
                VerticalPistonArm = Tag.FindGroupByTag<IMyPistonBase>(verticalTag, _grid, _bbuffer, _gbuffer);
                Drill = Tag.FindGroupByTag<IMyShipDrill>(drillTag, _grid, _bbuffer, _gbuffer);
                Rotor = Tag.FindBlockByTag<IMyMotorStator>(rotorTag, _grid, _bbuffer).FirstOrDefault();
                ReportLcd = Tag.FindBlockByTag<IMyTextPanel>(lcdTag, _grid, _bbuffer).FirstOrDefault();
                return HorizontalPistonArm.Any() && VerticalPistonArm.Any() && Drill.Any() && Rotor != null;
            }
        }
    }
}
