namespace Playground.Application.Common.Integration;

using Newtonsoft.Json;

public class IntegrationEventLogEntry
{
    private IntegrationEventLogEntry() { }

    public IntegrationEventLogEntry(IntegrationEvent integrationEvent, Guid transactionId)
    {
        EventId = integrationEvent.Id;
        CreationTime = integrationEvent.CreationDate;
        MessageJsonContent = JsonConvert.SerializeObject(integrationEvent);
        PublishState = EventState.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId.ToString();
        Type = integrationEvent.GetType();
    }

    public Guid EventId { get; init; }

    public IntegrationEvent IntegrationEvent { get; private set; }

    public EventState PublishState { get; private set; }

    public int TimesSent { get; private set; }

    public DateTime CreationTime { get; init; }

    public string MessageJsonContent { get; init; }

    public string TransactionId { get; init; }

    public Type Type { get; init; }

    public void IncreaseTimesSent() => TimesSent++;

    public void ChangePublishState(EventState state) => PublishState = state;

    public IntegrationEventLogEntry DeserializeJsonContent()
    {
        if (IntegrationEvent == null)
            IntegrationEvent = JsonConvert.DeserializeObject(MessageJsonContent, Type) as IntegrationEvent;

        return this;
    }
}