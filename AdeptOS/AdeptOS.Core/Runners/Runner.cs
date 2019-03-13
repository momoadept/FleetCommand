using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Utils;

namespace AdeptOS.Core.Runners
{
    public class Runner
    {
        public Runner(RunnerMetadata metadata)
        {
            Metadata = metadata;
        }

        public RunnerMetadata Metadata { get; private set; }

        public virtual void Run(string argument, UpdateType updateSource)
        {

        }
    }
}
