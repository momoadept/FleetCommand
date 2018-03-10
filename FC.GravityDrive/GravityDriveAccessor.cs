using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Core.Core.BlockReferences;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;

namespace FC.GravityDrive
{
    public class GravityDriveAccessor
    {
        public TagBlockReference<IMyGravityGenerator> Up;
        public TagBlockReference<IMyGravityGenerator> Dn;
        public TagBlockReference<IMyGravityGenerator> Lt;
        public TagBlockReference<IMyGravityGenerator> Rt;
        public TagBlockReference<IMyGravityGenerator> Fd;
        public TagBlockReference<IMyGravityGenerator> Bd;

        public TagBlockReference<IMyArtificialMassBlock> Mass;

        public TagBlockReference<IMyShipController> Controllers;

        public IEnumerable<BlockAccessor<IMyGravityGenerator>> AllGravity =>
            Up.Accessors
                .Concat(Dn.Accessors)
                .Concat(Lt.Accessors)
                .Concat(Rt.Accessors)
                .Concat(Fd.Accessors)
                .Concat(Bd.Accessors);

        public GravityDriveAccessor(IBlockReferenceFactory referenceFactory)
        {
            Up = referenceFactory.GetReference<IMyGravityGenerator>("GD U");
            Dn = referenceFactory.GetReference<IMyGravityGenerator>("GD D");
            Lt = referenceFactory.GetReference<IMyGravityGenerator>("GD L");
            Rt = referenceFactory.GetReference<IMyGravityGenerator>("GD R");
            Fd = referenceFactory.GetReference<IMyGravityGenerator>("GD F");
            Bd = referenceFactory.GetReference<IMyGravityGenerator>("GD B");

            Mass = referenceFactory.GetReference<IMyArtificialMassBlock>("GD M");
            Controllers = referenceFactory.GetReference<IMyShipController>("");

            TurnOff();
        }

        public void TurnOff()
        {
            Mass.ForEach(mass => mass.Enabled = false);
            foreach (var accessor in AllGravity)
            {
                accessor.Block.GravityAcceleration = 0;
                accessor.Block.Enabled = false;
            }
        }
        public void TurnOn()
        {
            Mass.ForEach(mass => mass.Enabled = true);
            foreach (var accessor in AllGravity)
            {
                accessor.Block.GravityAcceleration = 9.8f;
                accessor.Block.Enabled = false;
                // accessor.Block.FieldSize = new Vector3(2.5, 5, 2.5);
            }
        }
    }
}
