// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO;
using AnointedAutomation.SSO.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
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
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    })
    .AddAnointedAutomation(options =>
    {
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.CallbackPath = "/auth/anointed/callback"; // register https://<your host>/auth/anointed/callback in the partner portal
    });
builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapAnointedAutomationAuth("/auth/anointed");

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
