// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO;
using AnointedAutomation.SSO.UI;
using AnointedBlazorApp.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Set these with user-secrets in development (see README.md) and your secret store in production.
string clientId = Required(builder.Configuration, "Anointed:ClientId");
string clientSecret = Required(builder.Configuration, "Anointed:ClientSecret");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = AnointedAutomationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddAnointedAutomation(options =>
    {
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.CallbackPath = "/auth/anointed/callback"; // register https://<your host>/auth/anointed/callback in the partner portal
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapAnointedAutomationAuth("/auth/anointed");
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

static string Required(IConfiguration configuration, string key)
{
    string value = configuration[key];
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(key + " is not set. Run: dotnet user-secrets set \"" + key + "\" \"<value>\"");
    }
    return value;
}
