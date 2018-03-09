using System;
using System.Linq;
using FC.Core.Core;
using FC.Core.Core.BlockReferences;
using FC.Core.Core.ComponentModel;
using FC.Core.Core.FakeAsync.Promises;
using FC.Core.Core.Interfaces;
using FC.Core.Core.ServiceProvider;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript.ShipControls
{
    public partial class BaseShipControl : StatefullComponent<ShipControlState, IShipControlStrategy>, IService, IShipControl, IStatusReporter
    {
        public BaseShipControl() : base("ShipControl")
        {
            HasState(ShipControlState.Standby, new StandbyShipControl(this));
            HasState(ShipControlState.Moving, new MovingShipControl(this));
            HasState(ShipControlState.ManualOverride, new ManualOverrideShipControl(this));
        }

        public Type[] Provides { get; } = {typeof(IShipControl)};
        public virtual Promise MoveTo(Vector3D coords) => Strategy.MoveTo(coords);

        public Promise Stop()
        {
            return Promise.Resolve(() => State = ShipControlState.Standby);
        }

        public Promise<Vector3D?> GetCurrentWaypoint()
        {
            return Promise.FromValue(State == ShipControlState.Moving
                ? (Vector3D?) Autopilot.CurrentWaypoint.Coords
                : null);
        }

        public Promise SetManualOverride()
        {
            return Promise.Resolve(() =>  State = ShipControlState.ManualOverride);
        }

        public Promise RemoveManualOverride()
        {
            return Promise.Resolve(() =>  State = ShipControlState.Standby);
        }

        public string StatusEntityId => ComponentId;
        public int RefreshStatusDelay { get; } = 10;
        public string GetStatus()
        {
            switch (State)
            {
                case ShipControlState.Moving:
                    return $"Moving to {Autopilot.CurrentWaypoint.Coords}";
                case ShipControlState.Standby:
                    return "On standby";
                case ShipControlState.ManualOverride:
                    return "Manual Override";
            }

            return "wtf";
        }

        protected IBlockReferenceFactory BlockReferenceFactory;
        protected TagBlockReference<IMyRemoteControl> Autopilots;
        protected IMyRemoteControl Autopilot => Autopilots.Accessors.First().Block;
        protected override void OnAppBootstrapped(App app)
        {
            base.OnAppBootstrapped(app);

            BlockReferenceFactory = app.ServiceProvider.Get<IBlockReferenceFactory>();
            Autopilots = BlockReferenceFactory.GetReference<IMyRemoteControl>(ComponentId);
        }

        protected void ResetAutopilot()
        {
            Autopilot.Direction = Base6Directions.Direction.Forward;
            Autopilot.FlightMode = FlightMode.OneWay;
            Autopilot.ControlThrusters = true;
            Autopilot.ControlWheels = false;
            Autopilot.ClearWaypoints();
            Autopilot.SetAutoPilotEnabled(false);
            Autopilot.SetDockingMode(false);
            Autopilot.SetCollisionAvoidance(true);
        }
    }
}