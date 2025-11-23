using Microsoft.AspNetCore.SignalR;
using SenetServer.Shared;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SenetServer
{
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections =
            new(StringComparer.Ordinal);

        public override Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var userId = http is not null ? UserIdentity.GetOrCreateUserId(http) : null;

            if (!string.IsNullOrEmpty(userId))
            {
                var connections = _userConnections.GetOrAdd(userId, _ => new HashSet<string>());
                lock (connections)
                {
                    connections.Add(Context.ConnectionId);
                }
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception? exception)
        {
            foreach (var kvp in _userConnections)
            {
                var connections = kvp.Value;
                lock (connections)
                {
                    if (connections.Remove(Context.ConnectionId) && connections.Count == 0)
                    {
                        _userConnections.TryRemove(kvp.Key, out _);
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        public static IReadOnlyCollection<string> GetConnectionsForUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Array.Empty<string>();
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    return connections.ToArray();
                }
            }
            return Array.Empty<string>();
        }
    }
}
