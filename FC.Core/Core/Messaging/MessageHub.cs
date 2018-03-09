using System;
using System.Collections.Generic;
using FC.Core.Core.ComponentModel;
using FC.Core.Core.ServiceProvider;

namespace FC.Core.Core.Messaging
{
    public class MessageHub : BaseComponent, IMessageHub, IService, IMessageProcessor, IStatusReporter
    {
        protected Dictionary<string, IActionProvider> ActionProviders = new Dictionary<string, IActionProvider>();
        protected int MessagesProcessedTotal = 0;
        protected int MessagesProcessedSinceUpdate = 0;
        public MessageHub() : base("MessageHub")
        {
        }

        public void SubscribeToActionInvokations(IActionProvider actionProvider)
        {
            ActionProviders.Add(actionProvider.ActionProviderId, actionProvider);
        }

        public Type[] Provides { get; } = {typeof(IMessageHub)};
        public bool ProcessMessage(ComponentMessage message)
        {
            PlayerMessage playerMessage;
            if (PlayerMessage.TryParse(message.Text, out playerMessage))
            {
                message.StopProcessing = true;
                ProcessPlayerMessage(playerMessage);
                return true;
            }

            return false;
        }

        protected void ProcessPlayerMessage(PlayerMessage message)
        {
            GetActionProvider(message.ComponentId, message.Action)?.Invoke(message.Action, message.Args);
        }

        protected IActionProvider GetActionProvider(string componentId, string action)
        {
            if (!ActionProviders.ContainsKey(componentId))
            {
                Log.Warning($"No action provider with id {componentId}");
                return null;
            }

            if (!ActionProviders[componentId].CanInvoke(action))
            {
                Log.Warning($"Action provider {componentId} doesn't support action {action}");
                return null;
            }

            return ActionProviders[componentId];
        }

        public string StatusEntityId => ComponentId;
        public int RefreshStatusDelay { get; } = 1000;
        public string GetStatus()
        {
            var result =  $"Messages processed:\nLast {RefreshStatusDelay} ticks: {MessagesProcessedSinceUpdate}\nTotal: {MessagesProcessedTotal}";
            MessagesProcessedTotal = 0;
            return result;
        }
    }
}