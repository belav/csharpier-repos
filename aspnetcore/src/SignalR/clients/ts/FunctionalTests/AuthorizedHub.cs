using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FunctionalTests;

[Authorize(JwtBearerDefaults.AuthenticationScheme)]
public class HubWithAuthorization : Hub
{
    public string Echo(string message) => message;
}
