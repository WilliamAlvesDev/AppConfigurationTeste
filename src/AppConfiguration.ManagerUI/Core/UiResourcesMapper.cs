using AppConfiguration.ManagerUI.Configuration;
using AppConfiguration.ManagerUI.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppConfiguration.ManagerUI.Core
{
    internal class UiResourcesMapper
    {
        private readonly IUiResourcesReader _reader;

        public UiResourcesMapper(IUiResourcesReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void Map(IApplicationBuilder app, MapAppConfigurationOptions options)
        {
            var resources = _reader.UiResources;

            var uiResources = resources as UiResource[] ?? resources.ToArray();

            var ui = uiResources.GetMainUi(options);

            app.Map($"{options.UiPath}", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.OnStarting(() =>
                    {
                        if (!context.Response.Headers.ContainsKey("Cache-Control"))
                        {
                            context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                        }

                        return Task.CompletedTask;
                    });

                    context.Response.ContentType = ui.ContentType;

                    await context.Response.WriteAsync(ui.Content);
                });
            });
        }
    }
}
