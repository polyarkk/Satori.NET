using Satori.Protocol.Elements;
using Satori.Protocol.Events;

namespace Satori.Client;

public partial class SatoriBot {
    private readonly SatoriClient _client;

    public readonly string Platform;

    public readonly string SelfId;

    internal SatoriBot(SatoriClient client, string platform, string selfId) {
        _client = client;
        Platform = platform;
        SelfId = selfId;

        _client.EventService.EventReceived += EventRaiser;
    }

    private Task<TData> SendAsync<TData>(string endpoint, object? body) {
        return _client.ApiService.SendAsync<TData>(endpoint, Platform, SelfId, body);
    }

    private void EventRaiser(object? sender, Event e) {
        if (!e.Platform.Equals(Platform, StringComparison.OrdinalIgnoreCase) ||
            !e.SelfId.Equals(SelfId, StringComparison.OrdinalIgnoreCase))
            return;

        // 不处理自身发送的消息，防止出现死循环（e.g.: kook）
        if (e.SelfId == e.User?.Id) {
            return;
        }

        EventReceived?.Invoke(this, e);

        switch (e.Type) {
        case SatoriEventTypes.GuildAdded:
            GuildAdded?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildUpdated:
            GuildUpdated?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildRemoved:
            GuildRemoved?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildRequest:
            GuildRequest?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildMemberAdded:
            GuildMemberAdded?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildMemberUpdated:
            GuildMemberUpdated?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildMemberRemoved:
            GuildMemberRemoved?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildMemberRequest:
            GuildMemberRequest?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildRoleCreated:
            GuildRoleCreated?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildRoleUpdated:
            GuildRoleUpdated?.Invoke(this, e);
            break;

        case SatoriEventTypes.GuildRoleDeleted:
            GuildRoleDeleted?.Invoke(this, e);
            break;

        case SatoriEventTypes.LoginAdded:
            LoginAdded?.Invoke(this, e);
            break;

        case SatoriEventTypes.LoginRemoved:
            LoginRemoved?.Invoke(this, e);
            break;

        case SatoriEventTypes.LoginUpdated:
            LoginUpdated?.Invoke(this, e);
            break;

        case SatoriEventTypes.MessageCreated:
            MessageCreated?.Invoke(this, e);
            break;

        case SatoriEventTypes.MessageUpdated:
            MessageUpdated?.Invoke(this, e);
            break;

        case SatoriEventTypes.MessageDeleted:
            MessageDeleted?.Invoke(this, e);
            break;

        case SatoriEventTypes.ReactionAdded:
            ReactionAdded?.Invoke(this, e);
            break;

        case SatoriEventTypes.ReactionRemoved:
            ReactionRemoved?.Invoke(this, e);
            break;

        case SatoriEventTypes.FriendRequest:
            FriendRequest?.Invoke(this, e);
            break;
        }
    }
}
