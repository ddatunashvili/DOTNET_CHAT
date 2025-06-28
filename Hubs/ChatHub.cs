using Microsoft.AspNetCore.SignalR;
using ChatApp.Data;
using ChatApp.Models;
using System.Collections.Concurrent;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private static readonly ConcurrentDictionary<string, string> Users = new();

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            string username = Context.User?.Identity?.Name ?? Context.ConnectionId;
            Users[username] = Context.ConnectionId;

            await Clients.All.SendAsync("UpdateUserList", Users.Keys.ToList());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Context.User?.Identity?.Name ?? Context.ConnectionId;
            Users.TryRemove(username, out _);

            await Clients.All.SendAsync("UpdateUserList", Users.Keys.ToList());
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message, string receiver)
        {
            var sender = Context.User?.Identity?.Name ?? Context.ConnectionId;
            var timestamp = DateTime.UtcNow;

            var msg = new Message
            {
                SenderId = sender,
                ReceiverId = receiver,
                Content = message,
                Timestamp = timestamp,
                IsSeen = false
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            string formattedTime = timestamp.ToLocalTime().ToString("HH:mm");

            // Send to receiver
            if (Users.TryGetValue(receiver, out var receiverConnId))
            {
                await Clients.Client(receiverConnId).SendAsync("ReceiveMessage", sender, message, formattedTime);
            }

            // Send to sender (show in own chat window)
            if (Users.TryGetValue(sender, out var senderConnId))
            {
                await Clients.Client(senderConnId).SendAsync("ReceiveMessage", sender, message, formattedTime);
            }
        }

        public async Task Typing(string receiver)
        {
            var sender = Context.User?.Identity?.Name ?? Context.ConnectionId;
            if (Users.TryGetValue(receiver, out var receiverConnId))
            {
                await Clients.Client(receiverConnId).SendAsync("UserTyping", sender);
            }
        }

        public async Task StopTyping(string receiver)
        {
            var sender = Context.User?.Identity?.Name ?? Context.ConnectionId;
            if (Users.TryGetValue(receiver, out var receiverConnId))
            {
                await Clients.Client(receiverConnId).SendAsync("UserStopTyping", sender);
            }
        }

        public async Task SeenMessage(string receiver)
        {
            var currentUser = Context.User?.Identity?.Name ?? Context.ConnectionId;

            var unseenMessages = _context.Messages
                .Where(m => m.SenderId == receiver && m.ReceiverId == currentUser && !m.IsSeen)
                .ToList();

            if (unseenMessages.Any())
            {
                foreach (var msg in unseenMessages)
                {
                    msg.IsSeen = true;
                }

                await _context.SaveChangesAsync();

                if (Users.TryGetValue(receiver, out var senderConnId))
                {
                    await Clients.Client(senderConnId)
                        .SendAsync("MessageSeen", currentUser, receiver);
                }
            }
        }
    }
}
