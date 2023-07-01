using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("OpenIdConnect").AddCookie().AddOpenIdConnect();
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGet("/protected", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}!")
    .RequireAuthorization();

app.Run();
