namespace IngameScript.Core.Messaging
{
    // Player/one direction message: {component}.{action}.{args}
    // Grid message: r.{sender block id}.{target block id}.{message id}.{component}.{action}.{args}
    // Cross-grid message: c.{sender id}.{target id}.{message id}.{action}.{args}
    public interface IMessageProcessor
    {
        bool ProcessMessage(ComponentMessage message);
    }
}
