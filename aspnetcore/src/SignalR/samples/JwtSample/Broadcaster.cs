using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace JwtSample;

[Authorize(JwtBearerDefaults.AuthenticationScheme)]
public class Broadcaster : Hub
{
    public Task Broadcast(string sender, string message) =>
        Clients.All.SendAsync("Message", sender, message);
}
