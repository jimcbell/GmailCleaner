using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpLogging; //Http Logging Fields
using System.Net.Http.Headers; //MediaTypeWithQualityHeaderValue, AuthenticationHeaderValue
using System.Security.Claims; // ClaimTypes
using System.Text.Json; // JsonElement
using GmailCleaner.Adapters; // IEmailAdapter, ILoginAdapter, IHomeAdapter
using GmailCleaner.Repositories; // IEmailRepository, ITokenRepository, IUserRepository
using GmailCleaner.Common; // GmailCleanerContext
using GmailCleaner.Managers; // IAccessTokenManager, IUserManager
using GmailCleaner.Services; // IUserContextService, IGoogleRequestFactory
using GmailCleaner.Models.Settings; // GoogleApiSettings
using Azure.Identity; // DefaultAzureCredential
//using Azure.Security.KeyVault.Secrets; // SecretClient



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRazorPages();

// Add settings
GoogleApiSettings settings = builder.Configuration.GetSection("GoogleApiSettings").Get<GoogleApiSettings>() ?? new GoogleApiSettings();
builder.Services.AddSingleton<GoogleApiSettings>(settings);

// Get the database connection string from Key Vault
string keyVaultUrl = builder.Configuration["KeyVaultUrl"] ?? string.Empty;
//var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
//var connectionString = client.GetSecret("GmailCleanerConnectionString").Value.Value;

// Http Clients
builder.Services.AddHttpClient("google", c =>
{
    c.BaseAddress = new Uri("https://gmail.googleapis.com/gmail/v1");
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddHttpClient("google-auth", c =>
{
    c.BaseAddress = new Uri(settings.GoogleTokenUrl);
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Add azure key vault
builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());

// Get secret values required for startup
var connectionString = builder.Configuration["gmail-cleaner-db-connection-string"] ?? string.Empty;
var clientId = builder.Configuration["gmail-cleaner-client-id"] ?? string.Empty;
var clientSecret = builder.Configuration["gmail-cleaner-client-secret"] ?? string.Empty;
if (connectionString == string.Empty || clientId == string.Empty || clientSecret == string.Empty)
{
    throw new Exception("Missing required configuration values");
}

// Add database context based off connection string in azure keyvault
builder.Services.AddGmailCleanerContext(connectionString);

// Services
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IGoogleRequestFactory, GoogleRequestFactory>();


// Adapters
builder.Services.AddScoped<IEmailAdapter, EmailsAdapter>();
builder.Services.AddScoped<ILoginAdapter, LoginAdapter>();
builder.Services.AddScoped<IHomeAdapter, HomeAdapter>();



// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();

// Managers
builder.Services.AddScoped<IAccessTokenManager, AccessTokenManager>();
builder.Services.AddScoped<IUserManager, UserManager>();

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(10);
});


// Add authentication
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie")
    .AddOAuth("google", o =>
    {
        o.SignInScheme = "cookie";

        o.ClientId = clientId;
        o.ClientSecret = clientSecret;

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

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("cachePolicy", builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(100));
    });
});

//builder.Services.AddRateLimiter(options =>
//{
    
//});


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
app.UseOutputCache();

app.MapRazorPages().CacheOutput();

app.Run();
