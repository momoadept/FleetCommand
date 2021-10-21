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
        public interface ISequenceController
        {
            /// <summary>
            /// Queue a next step. Method call always adds a step to queue even if a step is already in progress.
            /// </summary>
            /// <returns>Promise that completes or fails when step is done</returns>
            IPromise<Void> StepOnce();

            IPromise<Void> Step(int times);

            /// <summary>
            /// </summary>
            /// <returns>Promise that completes when sequence is done, or fails</returns>
            IPromise<Void> StepAll();

            /// <summary>
            /// </summary>
            /// <returns>True if sequence is complete</returns>
            bool IsComplete();

            /// <summary>
            /// </summary>
            /// <returns>True if sequences has any steps queued to execute</returns>
            bool HasWork();

            /// <summary>
            /// </summary>
            /// <returns>True if step has started, but promise not yet resolved</returns>
            bool IsStepInProgress();

            /// <summary>
            /// </summary>
            /// <returns>True if next step can now begin (otherwise new steps will be added to queue)</returns>
            bool CanStepNow();

            /// <summary>
            /// </summary>
            /// <returns>True if at least one step was started</returns>
            bool IsStarted();

            /// <summary>
            /// </summary>
            /// <returns>True if paused</returns>
            bool IsPaused();

            /// <summary>
            /// Paused sequence won't start new steps until resumed, current active step allowed to continue
            /// </summary>
            void Pause();

            /// <summary>
            /// Resume sequence
            /// </summary>
            void Resume();

            /// <summary>
            /// Resets sequence to initial state. All pending steps will resolve with failed promise.
            /// </summary>
            void Reset();

            /// <summary>
            /// Removes all pending steps from queue
            /// </summary>
            void Interrupt();

            string GetReport();

            /// <summary>
            /// </summary>
            /// <returns>If steps throws an error, returns last exception</returns>
            Exception GetException();
        }
    }
}
