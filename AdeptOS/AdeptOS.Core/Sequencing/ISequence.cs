﻿using Sandbox.Game.EntityComponents;
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
        public interface ISequence
        {
            IPromise<Void> StepOnce();

            IPromise<Void> StepAll();

            bool IsComplete();

            bool HasWork();

            bool IsStepInProgress();

            bool CanStepNow();

            bool IsStarted();

            bool IsPaused();

            void Pause();

            void Resume();

            void Reset();

            void Interrupt();

            string GetReport();

            Exception GetException();
        }
    }
}
