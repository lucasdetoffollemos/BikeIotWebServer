using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BikeIotWebServer.Auth
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration)
            : base(options, logger, encoder)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var configuredApiKey = _configuration["Auth:ApiKey"];
            if (string.IsNullOrWhiteSpace(configuredApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("API key is not configured."));
            }

            if (!Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var providedApiKey))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!string.Equals(providedApiKey.ToString(), configuredApiKey, StringComparison.Ordinal))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "api-key-client"),
                new Claim(ClaimTypes.Name, "api-key-client")
            };

            var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationDefaults.AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.Headers.Append("WWW-Authenticate", ApiKeyAuthenticationDefaults.AuthenticationScheme);
            return Task.CompletedTask;
        }
    }
}
