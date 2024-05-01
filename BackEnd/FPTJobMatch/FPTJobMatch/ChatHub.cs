using FPT.Models;
using FPT.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Security.Claims;

namespace FPTJobMatch
{
    public class ChatHub : Hub
    {
        private static bool isAdminOnline = false;
        private static List<string> usersConnected = new List<string>(); // List of online users
        private static Queue<string> userQueue = new Queue<string>(); // Queue to store user IDs
        private static string? currentChattingUser = null; // Track the currently chatting user
        private readonly UserManager<ApplicationUser> _userManager;
        private string? _adminId; // Store the admin user

        public ChatHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            InitializeAdminUser();
        }
        private void InitializeAdminUser()
        {
            ApplicationUser adminUser = _userManager.FindByEmailAsync("minhbee203@gmail.com").Result;
            _adminId = adminUser?.Id;
        }

        public override async Task OnConnectedAsync()
        {
            // Get user ID from context
            string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == _adminId)
            {
                isAdminOnline = true;
            }
            usersConnected.Add(userId);

            if (userId == _adminId && currentChattingUser != null)
            {
                await Clients.User(currentChattingUser).SendAsync("ReceiveMessage", "You are now chatting with the admin.");
                await Clients.User(userId).SendAsync("ReceiveMessage", "You are now chatting with the user.");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == _adminId)
            {
                isAdminOnline = false;
            }
            usersConnected.Remove(userId);

            await base.OnDisconnectedAsync(exception);
        }

        public bool IsAdminOnline()
        {
            return usersConnected.Contains(_adminId);
        }


        public async Task AddToQueue(string userId)
        {
            if (currentChattingUser == null)
            {
                currentChattingUser = userId;
                if (isAdminOnline)
                {
                    await Clients.User(currentChattingUser).SendAsync("ReceiveMessage", "You are now chatting with the admin.");
                    await Clients.User(_adminId).SendAsync("ReceiveMessage", "You are now chatting with a new user.");
                }
            }
            else
            {
                if (!userQueue.Contains(userId))
                {
                    userQueue.Enqueue(userId);
                }
                await UpdateChatNumber(userId);
            }
        }

        public async Task RemoveFromQueue(string userId)
        {
            if (currentChattingUser == userId)
            {
                currentChattingUser = null;
                if (userQueue.Any())
                {
                    currentChattingUser = userQueue.Dequeue();

                    await Clients.User(currentChattingUser).SendAsync("ReceiveMessage", "You are now chatting with the admin.");
                    await Clients.User(_adminId).SendAsync("ReceiveMessage", "You are now chatting with a new user.");

                    //if (userQueue.Any())
                    //{
                    //    await Clients.User(_adminId).SendAsync("ReceiveMessage", "You are now chatting with a new user.");
                    //}
                    //else
                    //{
                    //    await Clients.User(_adminId).SendAsync("ReceiveMessage", "There are no users left.");
                    //}
                } else
                {
                    await Clients.User(_adminId).SendAsync("ReceiveMessage", "There are no users left.");
                }
            }
            else
            {
                if (userQueue.Contains(userId))
                {
                    userQueue = new Queue<string>(userQueue.Where(u => u != userId));
                    foreach (var user in userQueue)
                    {
                        await UpdateChatNumber(user);
                    }
                }
            }
        }

        private async Task UpdateChatNumber(string userId)
        {
            int chatNumber = userQueue.ToList().IndexOf(userId) + 1;
            await Clients.User(userId).SendAsync("UpdateChatNumber", chatNumber);
        }

        // Method to send message to admin
        public async Task SendMessage(string message)
        {
            if (usersConnected.Contains(_adminId))
            {
                await Clients.User(_adminId).SendAsync("ReceiveMessage", message);
            } else
            {
                await Clients.User(currentChattingUser).SendAsync("ReceiveMessage", "Admin is not online. Message not sent.");
            }
        }

        // Method to send message to user (for admin)
        public async Task SendMessageToUser(string message)
        {
            if (currentChattingUser == null)
            {
                await Clients.User(_adminId).SendAsync("ReceiveMessage", "There are currently no users to chat with");
            } else
            {
                await Clients.User(currentChattingUser).SendAsync("ReceiveMessage", message);
            }
        }

    }
}
