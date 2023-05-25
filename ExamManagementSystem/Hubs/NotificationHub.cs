using ExamManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ExamManagementSystem.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly static Dictionary<string, string> _connections = new();

        public override Task OnConnectedAsync()
        {
            string userMail = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            if (!_connections.ContainsKey(userMail))
            {
                _connections.Add(userMail, connectionId);
            }
            else
            {
                _connections[userMail] = connectionId;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string userMail = Context.User.Identity.Name;

            if (_connections.ContainsKey(userMail))
            {
                _connections.Remove(userMail);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task NotifyExamStatus(string userMail, Exam exam)
        {
            if (_connections.TryGetValue(userMail, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("NotifyExamStatus", exam);
            }
            else
            {
                // handle the case where the user is not currently connected
            }
        }
    }
}
