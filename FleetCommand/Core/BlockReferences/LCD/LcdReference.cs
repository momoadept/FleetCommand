using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.Core.BlockLoader;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Core.BlockReferences.LCD
{
    public class LcdReference: TagBlockReference<IMyTextPanel>
    {
        public void SetText(string value)
        {
            GetMyBlocks().ForEach(screen => { screen.WritePublicText(value); });
        }

        public void Append(string value)
        {
            GetMyBlocks().ForEach(screen => { screen.WritePublicText(value, true); });
        }

        public void Clear()
        {
            GetMyBlocks().ForEach(screen => { screen.WritePublicText(""); });
        }

        public void Init()
        {
            GetMyBlocks().ForEach(screen => { screen.ShowPublicTextOnScreen(); });
            Clear();
        }

        public LcdReference(IBlockLoader blocks, string tag) : base(blocks, tag)
        {
            Init();
        }
    }
}
