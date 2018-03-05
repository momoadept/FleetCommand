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
            Prepare();
            GetMyBlocks().ForEach(screen => { screen.WritePublicText(value); });
        }

        public void Append(string value)
        {
            Prepare();
            GetMyBlocks().ForEach(screen => { screen.WritePublicText(value, true); });
        }

        public void Clear()
        {
            Prepare();
            GetMyBlocks().ForEach(screen => { screen.WritePublicText(""); });
        }

        public void Prepare()
        {
            GetMyBlocks().ForEach(screen => { screen.ShowPublicTextOnScreen(); });
        }

        public LcdReference(string tag) : base(tag)
        {
            Prepare();
        }
    }
}
