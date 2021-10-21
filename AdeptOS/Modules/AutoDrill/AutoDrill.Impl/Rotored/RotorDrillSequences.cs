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
            public StepSequence DrillLayer;
            public StepSequence LowerToLayer;
            public StepSequence RewindHorizontalDrill;
            public StepSequence RewindVerticalDrill;
            public StepSequence RewindRotor;

            private float _baseSpeed = 0.5f;
            private float _layerHeight = 2f;
            private float _baseStep = 1f;

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

                var lowerDrillSteps = new ExtendContractPistonArm(
                    blocks.VerticalPistonArm.ToArray(),
                    _baseSpeed,
                    "V Drill",
                    maxStep * blocks.VerticalPistonArm.Count,
                    _layerHeight
                );

                var lowerDrillOnce = new SkipStepper(lowerDrillSteps.Stepper(), "lower drill once", 1);

                var liftDrillSteps = new ExtendContractPistonArm(
                    blocks.VerticalPistonArm.ToArray(),
                    -_baseSpeed,
                    "V Drill",
                    0,
                    _layerHeight
                );

                var extendArmStep = new ExtendContractPistonArm(
                    blocks.HorizontalPistonArm.ToArray(),
                    _baseSpeed,
                    "Extend arm",
                    maxStep * blocks.HorizontalPistonArm.Count,
                    _baseStep);

                var contractArmStep = new ExtendContractPistonArm(
                    blocks.HorizontalPistonArm.ToArray(),
                    -_baseSpeed,
                    "Contract arm",
                    0,
                    _baseStep);

                Func<float> rotationSpeed = () =>
                {
                    var metersPerM = _baseSpeed * 60;
                    var radius = (blocks.Drill.GetPosition() - blocks.Rotor.GetPosition()).Length();
                    var circ = 2 * Math.PI * radius;

                    var rpm = metersPerM / circ;
                    return (float)rpm;
                };

                var rotor = new RotateRotorSequence(
                    blocks.Rotor,
                    5,
                    rotationSpeed,
                    1,
                    rotorMin,
                    rotorMax);

                var rewindRotor = rotor.RewindStepper();

                var rotorSteps = rotor.Stepper();

                var digForths = extendArmStep.Stepper();
                var digBacks = contractArmStep.Stepper();

                var resetExtender = new SequenceStep()
                {
                    StepTag = "reset ex",
                    PromiseGenerator = () =>
                    {
                        digForths.Reset();
                        rotorSteps.Reset();
                        return Void.Promise();
                    }
                };

                var resetContractor = new SequenceStep()
                {
                    StepTag = "reset ct",
                    PromiseGenerator = () =>
                    {
                        digBacks.Reset();
                        rotorSteps.Reset();
                        return Void.Promise();
                    }
                };

                var digForth = new PairStepper(new PairStepper(digForths, new UnitStepper(resetExtender)), new SkipStepper(rotorSteps, "rotate rotor", 1));
                var digBack = new PairStepper(new PairStepper(digBacks, new UnitStepper(resetContractor)), new SkipStepper(rotorSteps, "rotate rotor", 1));

                var digBackAndForth = new PairStepper(digForth, digBack);
                var digCycle = new CycleStepper(digBackAndForth, () => !blocks.Rotor.AngleDeg().AlmostEquals(rotorMax));

                DrillLayer = new StepSequence(digCycle);
                LowerToLayer = new StepSequence(lowerDrillSteps.Stepper());
                RewindHorizontalDrill = new StepSequence(contractArmStep.Stepper());
                RewindVerticalDrill = new StepSequence(liftDrillSteps.Stepper());
                RewindRotor = new StepSequence(rewindRotor);
            }
        }
    }
}
