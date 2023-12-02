using GmailCleaner.Models.Settings;
using GmailCleaner.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpLogging;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using GmailCleaner.Adapters;
using GmailCleaner.Repositories;
using GmailCleaner.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRazorPages();

// Add settings
GoogleApiSettings settings = builder.Configuration.GetSection("GoogleApiSettings").Get<GoogleApiSettings>() ?? new GoogleApiSettings();
builder.Services.AddSingleton<GoogleApiSettings>(settings);

// Register Http Clients
builder.Services.AddHttpClient("google", c =>
{
    c.BaseAddress = new Uri("https://gmail.googleapis.com/gmail/v1");
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Custom services
builder.Services.AddScoped<IGoogleRequestFactory, GoogleRequestFactory>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IEmailAdapter, EmailsAdapter>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<ILoginAdapter, LoginAdapter>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

string connectionString = builder.Configuration.GetConnectionString("GmailCleaner") ?? string.Empty;
builder.Services.AddGmailCleanerContext(connectionString);


// Add authentication
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie")
    .AddOAuth("google", o =>
    {
        o.SignInScheme = "cookie";

        o.ClientId = settings.ClientId;
        o.ClientSecret = settings.ClientSecret;

        o.AuthorizationEndpoint = settings.GoogleAuthUrl;
        o.TokenEndpoint = settings.GoogleTokenUrl;
        o.UserInformationEndpoint = settings.GoogleUserInfoUrl;
        o.CallbackPath = "/auth";
        o.SaveTokens = true;

        o.ClaimActions.MapJsonKey("id", "sub");
        o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        o.Scope.Add("https://mail.google.com/ https://www.googleapis.com/auth/userinfo.profile openid");
        //o.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");

        o.Events.OnCreatingTicket = async ctx =>
        {
            var scopes = ctx.Properties.GetParameter<ICollection<string>>(OAuthChallengeProperties.ScopeKey);
            var properties = ctx.Properties;
            var tokens = ctx.TokenResponse;
            using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
            try
            {
                using var result = await ctx.Backchannel.SendAsync(request);
                var user = await result.Content.ReadFromJsonAsync<JsonElement>();
                var text = await result.Content.ReadAsStringAsync();
                ctx.RunClaimActions(user);
                ctx.HttpContext.Response.Headers.Append("Token-Exp", ctx.ExpiresIn.ToString());
                ctx.HttpContext.Request.Headers.Append("Token-Exp", ctx.ExpiresIn.ToString());
            }
            catch { }
        };
        
    });

// Add logging
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("Authorization");
    logging.ResponseHeaders.Add("Authorization");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var app = builder.Build();
if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHttpLogging();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.MapGet("/claims",
    (HttpContext ctx) =>
    {
        ctx.GetTokenAsync("access_token");
        return ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList();
    }
);
app.MapGet("/login", () =>
{
    return Results.Challenge(
        new AuthenticationProperties() { RedirectUri = "/loginsuccess" },
        authenticationSchemes: new List<string>() { "google" });
});

app.MapRazorPages();

app.Run();
