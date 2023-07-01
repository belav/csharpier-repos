using Microsoft.AspNetCore.Authorization;

namespace Microsoft.AspNetCore.SignalR.Tests;

[Authorize]
class AuthHub : Hub { }
