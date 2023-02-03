using AppConfiguration.Manager;
using AppConfiguration.ManagerUI.Configuration;
using AppConfiguration.ManagerUI.Core;
using AppConfiguration.ManagerUI.Core.Extensions;
using Ardalis.SmartEnum.SystemTextJson;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;

namespace AppConfiguration.ManagerUI.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapAppConfigurationUi(this IEndpointRouteBuilder builder, Action<MapAppConfigurationOptions> setup = null)
        {
            var embeddedResourcesAssembly = typeof(UiResource).Assembly;

            var options = new MapAppConfigurationOptions();
            setup?.Invoke(options);

            var featureFlagService = builder.ServiceProvider.GetRequiredService<IFeatureFlagService>();
            var featureSettingService = builder.ServiceProvider.GetRequiredService<IFeatureSettingService>();

            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new SmartEnumNameConverter<FeatureFlagStatus, int>()
                }
            };

            const string apiNamespace = "api";

            builder.MapGet("AppConfiguration", async context =>
            {
                var resources = new UiEmbeddedResourcesReader(embeddedResourcesAssembly);

                var ui = resources.UiResources.GetMainUi(new MapAppConfigurationOptions
                {
                    ApiNamespace = options.ApiNamespace
                });

                context.Response.ContentType = ui.ContentType;
                await context.Response.WriteAsync(ui.Content);
            });

            builder.MapGet($"{apiNamespace}/AppConfiguration/FeatureFlags", async context =>
            {
                var featureFlags = await featureFlagService.RetrieveAsync(default).ConfigureAwait(false);

                context.Response.ContentType = ContentType.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(featureFlags.Select(f => new FeatureFlagResult(f)), serializeOptions));
            });

            builder.MapGet($"{apiNamespace}/AppConfiguration/FeatureFlags/{{name}}", async context =>
            {
                var name = $"{context.Request.RouteValues["name"]}";

                var featureFlag = await featureFlagService.RetrieveAsync(name, default)
                    .ConfigureAwait(false);

                context.Response.ContentType = ContentType.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new FeatureFlagResult(featureFlag), serializeOptions));
            });

            builder.MapPost($"{apiNamespace}/AppConfiguration/FeatureFlags/{{name}}/Toggle", async context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var user = ValidateToken(token);

                if (user is null)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var name = $"{context.Request.RouteValues["name"]}";

                var featureFlag = await featureFlagService
                    .SetUser(user)
                    .ToggleAsync(name, default)
                    .ConfigureAwait(false);

                context.Response.ContentType = ContentType.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new FeatureFlagResult(featureFlag), serializeOptions));
            });

            builder.MapGet($"{apiNamespace}/AppConfiguration/Settings", async context =>
            {
                var featureSettings = await featureSettingService.RetrieveAsync(default).ConfigureAwait(false);

                context.Response.ContentType = ContentType.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(featureSettings, serializeOptions));
            });

            builder.MapGet($"{apiNamespace}/AppConfiguration/Settings/{{name}}", async context =>
            {
                var name = $"{context.Request.RouteValues["name"]}";

                var featureSetting = await featureSettingService.RetrieveAsync(name, default)
                    .ConfigureAwait(false);

                context.Response.ContentType = ContentType.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(featureSetting, serializeOptions));
            });

            builder.MapPost($"{apiNamespace}/AppConfiguration/Settings/{{name}}/{{value}}", async context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var user = ValidateToken(token);

                if (user is null)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var name = $"{context.Request.RouteValues["name"]}";
                var value = $"{context.Request.RouteValues["value"]}";

                var featureSetting = await featureSettingService
                    .SetUser(user)
                    .ChangeValueAsync(name, value, default)
                    .ConfigureAwait(false);

                context.Response.ContentType = ContentType.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(featureSetting, serializeOptions));
            });

            builder.MapPost($"{apiNamespace}/AppConfiguration/Settings/{{name}}/", async context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var user = ValidateToken(token);

                if (user is null)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var name = $"{context.Request.RouteValues["name"]}";

                var featureSetting = await featureSettingService
                    .SetUser(user)
                    .ChangeValueAsync(name, null, default)
                    .ConfigureAwait(false);

                context.Response.ContentType = ContentType.Json;
                await context.Response.WriteAsync(JsonSerializer.Serialize(featureSetting, serializeOptions));
            });

            return builder;
        }

        public static User ValidateToken(string token)
        {
            try
            {
                var jwt = token.Replace("Bearer ", string.Empty, StringComparison.InvariantCultureIgnoreCase);

                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

                if (!(jwtSecurityTokenHandler.ReadToken(jwt) is JwtSecurityToken jwtSecurityToken))
                {
                    return null;
                }

                if (DateTime.Now > jwtSecurityToken.ValidTo)
                {
                    return null;
                }

                var claims = jwtSecurityToken.Claims.ToArray();

                var name = claims.First(claim => claim.Type == "name").Value;
                var email = claims.First(claim => claim.Type == "unique_name").Value;

                return new User
                {
                    Name = name,
                    Email = email
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}