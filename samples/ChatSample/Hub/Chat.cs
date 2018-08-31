// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatSample
{
    public class Chat : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var request = Context.GetHttpContext().Request;
            if (request != null)
            {
                foreach (var header in request.Headers)
                {
                    await Clients.Caller.SendAsync("echo", "SYSTEM", $"Header: {header}");
                }
                await Clients.Caller.SendAsync("echo", "SYSTEM", $"Path: {request.Path}");
                await Clients.Caller.SendAsync("echo", "SYSTEM", $"Query: {request.QueryString}");
            }
            await base.OnConnectedAsync();
        }

        public void BroadcastMessage(string name, string message)
        {
            Clients.All.SendAsync("broadcastMessage", name, message);
        }

        public async Task Echo(string name, string message)
        {
            const string groupName = "some-group";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("echo", name, message + "(echo from server)");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
