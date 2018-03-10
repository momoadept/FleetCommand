using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Core.Core;
using FC.Core.Core.BlockReferences;
using FC.Core.Core.ComponentModel;
using FC.Core.Core.FakeAsync;
using FC.Core.Core.FakeAsync.Promises;
using FC.Core.Core.Interfaces;
using FC.Core.Core.Messaging;
using FC.Core.Core.ServiceProvider;
using VRageMath;

namespace FC.GravityDrive
{
    public class GravityDrive: BaseComponent, IGravityDrive, IService, IStatusReporter, IActionProvider
    {
        public Type[] Provides { get; } = {typeof(IGravityDrive)};
        public string StatusEntityId => ComponentId;
        public int RefreshStatusDelay => 100;

        protected OnOff State = OnOff.Off;
        protected SimpleAsyncWorker GravityDriveWorker;
        protected GravityDriveAccessor Drive;
        protected double Strength = 1;
        protected Vector3 DetectedInput = Vector3.Invalid;
        protected ActionDescriptor Actions = new ActionDescriptor();

        protected override void OnAppBootstrapped(App app)
        {
            base.OnAppBootstrapped(app);

            Drive = new GravityDriveAccessor(app.ServiceProvider.Get<IBlockReferenceFactory>());
            GravityDriveWorker = new SimpleAsyncWorker("GravityDriveWorker", UpdateGravityVectors);
            Async.AddJob(GravityDriveWorker);
            app.ServiceProvider.Get<IMessageHub>().SubscribeToActionInvokations(this);
            Drive.TurnOff();
        }

        public GravityDrive() : base("GravityDrive")
        {
            Actions.HasAction("Enable", args => Enable());
            Actions.HasAction("Disable", args => Disable());
            Actions.HasAction("SetStrength", args => SetStrength(double.Parse(args[0])));
        }

        public Promise<OnOff> GetState() => Promise.FromValue(State);

        public Promise Enable()
        {
            if (!CheckHealth())
            {
                var e = new Exception("Cannot enable gravity drive: blocks missing");
                Log.Warning(e.Message);
                return Promise.FromError(e);
            }

            return Promise.Resolve(() =>
            {
                State = OnOff.On;
                Drive.TurnOn();
                GravityDriveWorker.Start();
                Log.Info("Gravity Drive enabled");
            });
        }

        public Promise Disable()
        {
            return Promise.Resolve(() =>
            {
                State = OnOff.Off;
                Drive.TurnOff();
                GravityDriveWorker.Stop();
                Log.Info("Gravity Drive disabled");
            });
        }

        public Promise SetStrength(double value)
        {
            return Promise.Resolve(() => { Strength = value; });
        }


        public string GetStatus()
        {
            if (State == OnOff.On)
            {
                return $"Online\nDetected Acc: {DetectedInput}\nControllers count: {Drive.Controllers.Accessors.Count}";
            }

            return "Offline";
        }

        protected void UpdateGravityVectors()
        {
            if (!CheckHealth())
            {
                var e = new Exception("Terminating gravity drive: blocks missing");
                Log.Warning(e.Message);
                Disable();
            }

            var inputBlock = Drive.Controllers.Accessors.Select(ctrl => ctrl.Block)
                .FirstOrDefault(b => !b.MoveIndicator.Equals(Vector3.Zero));

            if (inputBlock == null)
            {
                HandleInertiaDampening();
                return;
            }

            DetectedInput = inputBlock.MoveIndicator;
            DetectedInput.Normalize();

            Drive.Bd.ForEach(g => g.Enabled = DetectedInput.Z > 0);
            Drive.Dn.ForEach(g => g.Enabled = DetectedInput.Y < 0);
            Drive.Fd.ForEach(g => g.Enabled = DetectedInput.Z < 0);
            Drive.Lt.ForEach(g => g.Enabled = DetectedInput.X < 0);
            Drive.Rt.ForEach(g => g.Enabled = DetectedInput.X > 0);
            Drive.Up.ForEach(g => g.Enabled = DetectedInput.Y > 0);
        }

        protected void HandleInertiaDampening()
        {
            DetectedInput = Vector3.Zero;

            Drive.Bd.ForEach(g => g.Enabled = DetectedInput.Z > 0);
            Drive.Dn.ForEach(g => g.Enabled = DetectedInput.Y < 0);
            Drive.Fd.ForEach(g => g.Enabled = DetectedInput.Z < 0);
            Drive.Lt.ForEach(g => g.Enabled = DetectedInput.X < 0);
            Drive.Rt.ForEach(g => g.Enabled = DetectedInput.X > 0);
            Drive.Up.ForEach(g => g.Enabled = DetectedInput.Y > 0);
        }

        protected bool CheckHealth()
        {
            var ok = true;

            if (!Drive.Up.Accessors.Any(a => a.Block.IsFunctional))
            {
                Log.Warning("No upward thrust gravity generator");
                ok = false;
            }
            if (!Drive.Dn.Accessors.Any(a => a.Block.IsFunctional))
            {
                Log.Warning("No downward thrust gravity generator");
                ok = false;
            }
            if (!Drive.Lt.Accessors.Any(a => a.Block.IsFunctional))
            {
                Log.Warning("No left thrust gravity generator");
                ok = false;
            }
            if (!Drive.Rt.Accessors.Any(a => a.Block.IsFunctional))
            {
                Log.Warning("No right thrust gravity generator");
                ok = false;
            }
            if (!Drive.Fd.Accessors.Any(a => a.Block.IsFunctional))
            {
                Log.Warning("No forward thrust gravity generator");
                ok = false;
            }
            if (!Drive.Bd.Accessors.Any(a => a.Block.IsFunctional))
            {
                Log.Warning("No backward thrust gravity generator");
                ok = false;
            }

            if (!Drive.Mass.Accessors.Any(a => a.Block.IsFunctional))
            {
                Log.Warning("No mass block");
                ok = false;
            }

            if (!Drive.Controllers.Accessors.Any(a => a.Block.IsFunctional))
            {
                Log.Warning("No ship controls");
                ok = false;
            }

            return ok;
        }

        public Promise<string> Invoke(string action, string[] args)
        {
            return Actions.Invoke(action, args);
        }

        public bool CanInvoke(string action)
        {
            return Actions.CanInvoke(action);
        }

        public string ActionProviderId => ComponentId;
    }
}
