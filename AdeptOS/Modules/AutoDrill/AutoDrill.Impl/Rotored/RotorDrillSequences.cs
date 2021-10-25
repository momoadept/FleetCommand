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
        public class RotorDrillSequences
        {
            public SequenceController DrillLayer;
            public SequenceController LowerToLayer;
            public SequenceController RewindHorizontalDrill;
            public SequenceController RewindVerticalDrill;
            public SequenceController RewindRotor;

            private float _baseSpeed = 0.5f;
            private float _layerHeight = 2f;
            private float _baseStep = 3f;

            public void Build(RotorDrillBlocks blocks)
            {
                var maxStep = blocks.VerticalPistonArm.First().HighestPosition;
                var terms = blocks.Rotor.CustomData.Trim().Split(',');
                float rotorMax = 360;
                float rotorMin = 0;
                if (terms.Length >= 2)
                {
                    rotorMin = float.Parse(terms[0]);
                    rotorMax = float.Parse(terms[1]);
                }

                Func<float> rotationSpeed = () =>
                {
                    var metersPerM = _baseSpeed * 60;
                    var radius = (blocks.Drill.First().GetPosition() - blocks.Rotor.GetPosition()).Length();
                    var circ = 2 * Math.PI * radius;

                    var rpm = metersPerM / circ;
                    return (float)rpm;
                };

                var lowerDrill = new ExtendContractPistonArm(
                    blocks.VerticalPistonArm.ToArray(),
                    _baseSpeed,
                    "V Drill",
                    maxStep * blocks.VerticalPistonArm.Count,
                    _layerHeight
                ).Stepper();

                var liftDrill = new ExtendContractPistonArm(
                    blocks.VerticalPistonArm.ToArray(),
                    -_baseSpeed*3,
                    "V Drill",
                    0,
                    _layerHeight
                ).Stepper();

                var extendArm = new ExtendContractPistonArm(
                    blocks.HorizontalPistonArm.ToArray(),
                    _baseSpeed,
                    "Extend arm",
                    maxStep * blocks.HorizontalPistonArm.Count,
                    _baseStep).Stepper();

                var contractArm = new ExtendContractPistonArm(
                    blocks.HorizontalPistonArm.ToArray(),
                    -_baseSpeed*3,
                    "Contract arm",
                    0,
                    _baseStep*2).Stepper();

                var rotor = new RotateRotorSequence(
                    blocks.Rotor,
                    5,
                    rotationSpeed,
                    1,
                    rotorMin,
                    rotorMax);

                var rewindRotor = rotor.RewindStepper();

                var rotateRotor = rotor.Stepper();

                var traceableExtendArm = extendArm.New();

                var drillLayer = rotateRotor.New()
                    .ContinueWith(rewindRotor.New())
                    .ContinueWith(traceableExtendArm.NumberOfSteps(1))
                    .RepeatWhile(() => !traceableExtendArm.IsComplete())
                    .ContinueWith(rotateRotor.New()) // Last full rotation
                    .DoAfter((Action)(() => traceableExtendArm.Reset()))
                    .DoAfter((Action)(() => blocks.SetDrills(true)))
                    .ContinueWith(contractArm.New())
                    .ContinueWith(rewindRotor.New());

                var lowerToLayer = lowerDrill.New();
                var rewindVertical = liftDrill.New();
                var rewindHorizontal = contractArm.New();

                DrillLayer = new SequenceController(drillLayer);
                LowerToLayer = new SequenceController(lowerToLayer);
                RewindHorizontalDrill = new SequenceController(rewindHorizontal);
                RewindVerticalDrill = new SequenceController(rewindVertical);
                RewindRotor = new SequenceController(rewindRotor);
            }
        }
    }
}
