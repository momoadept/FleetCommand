﻿using Sandbox.ModAPI.Ingame;
using System;

namespace IngameScript
{
    partial class Program
    {
        public class GameContext: IGameContext, IModule
        {
            MyGridProgram _root;
            Action<string> _storageSetter;

            public GameContext(MyGridProgram root, Action<string> storageSetter)
            {
                _root = root;
                _storageSetter = storageSetter;
            }

            public void Echo(string s) => _root.Echo(s);

            public int CurrentSteps => _root.Runtime.CurrentInstructionCount;
            public int MaxSteps => _root.Runtime.MaxInstructionCount;
            public string Storage
            {
                get { return _root.Storage; }
                set { _storageSetter(value); }
            }

            public IMyGridTerminalSystem Grid => _root.GridTerminalSystem;
            public IMyProgrammableBlock Me => _root.Me;

            public string UniqueName => "Root";
            public string Alias => "Root";

            public void Bind(IBindingContext context)
            {
            }

            public void Run()
            {
            }

            public void OnSaving()
            {
            }
        }
    }
}
