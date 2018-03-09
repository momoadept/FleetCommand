using System;
using FC.Core.Core;
using FC.ShipControls;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    internal class Program : MyGridProgram
    {
        public Program()
        {
            try
            {
                App.GlobalConfiguration = new AppConfig
                {
                    EnableMasterLog = true
                };

                MyApp = new App("TestShip", this)
                    .BootstrapComponents(new BaseShipControl());
                MyApp.Start();
            }
            catch (Exception e)
            {
                Echo(e.Message + e.StackTrace);
            }
        }

        public App MyApp { get; set; }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            MyApp?.Tick(argument, updateSource);
        }
    }
}