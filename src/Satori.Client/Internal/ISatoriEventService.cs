using Satori.Protocol.Events;

namespace Satori.Client.Internal;

internal interface ISatoriEventService : IDisposable {
    event EventHandler<Event> EventReceived;

    Task StartAsync();

    Task StopAsync();
}
