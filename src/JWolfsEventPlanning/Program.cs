using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
const string TelegramScheme = "Telegram";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = TelegramScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
    })
    .AddOpenIdConnect(TelegramScheme, options =>
    {
        options.Authority = "https://oauth.telegram.org";

        options.ClientId = builder.Configuration["Authentication:Telegram:ClientId"]
            ?? throw new InvalidOperationException("Missing Authentication:Telegram:ClientId");

        options.ClientSecret = builder.Configuration["Authentication:Telegram:ClientSecret"]
            ?? throw new InvalidOperationException("Missing Authentication:Telegram:ClientSecret");

        options.ResponseType = OpenIdConnectResponseType.Code;
        options.UsePkce = true;

        // Telegram OIDC discovery document:
        // https://oauth.telegram.org/.well-known/openid-configuration
        options.MetadataAddress = "https://oauth.telegram.org/.well-known/openid-configuration";

        options.CallbackPath = "/signin-telegram";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");

        // Optional:
        // options.Scope.Add("phone");
        // options.Scope.Add("telegram:bot_access");

        // Telegram currently returns user claims in the ID token.
        // It does not currently provide a separate UserInfo endpoint.
        options.GetClaimsFromUserInfoEndpoint = false;

        options.SaveTokens = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://oauth.telegram.org",
            NameClaimType = "name"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();

app.Run();
