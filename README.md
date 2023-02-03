# AppConfiguration.Manager

Este repositório oferece um pacote para gerenciamento de FeatureFlags/Settings integrados com Azure App Configuration.

## Instalação

```PowerShell
Install-Package AppConfiguration.Manager
```

Assim que o pacote estiver instalado, você pode adicionar o AppConfiguration Manager usando o método de extensão ** AddAppConfiguration ** do IServiceCollection.

```csharp
using AppConfiguration.Manager;

public void ConfigureServices(IServiceCollection services)
{
    services.AddAppConfiguration(options =>
		{
			options.Namespace = "AppConfiguration";
		});
}
```

As configurações possíveis:
* Namespace: Será utilizado como prefixo na chaves
* UseDistributedCache: Define se utilizará cache distribuido (Redis): _Default: true_
* CacheExpiration: Define o time to live do cache. _Default: 15 segundos (Se TimeSpan.Zero, é o mesmo que UseDistributedCache: false)_
* AzureAppConfigurationConnectionStringKey: Define a chave da connection string do Azure AppConfiguration a ser definida no arquivo appsettings.json. _Default: AzureAppConfiguration_
```json
"ConnectionStrings": {
    "AzureAppConfiguration": "...", //Importante que seja uma connection string com permissão de escrita
},
```  
* RedisConnectionStringKey: Define a chave da connection string do Redis a ser definida no arquivo appsettings.json. _Default: Redis_
```json
"ConnectionStrings": {
    "Redis": "...",
},
```  

Para registrar uma FeatureFlag ou Setting basta criar uma classe que implemente a interface ** IAppConfigurationSettings **

```csharp
using AppConfiguration.Manager;

public sealed class AppConfigurationSettings : IAppConfigurationSettings
{
    public static FeatureFlag XpSinacorIntegration =>
                new FeatureFlag(nameof(XpSinacorIntegration), "Feature flag responsavel por habilitar a integração como o Sinacor da XP", false);
                
    public static FeatureSetting ComplianceAnalysisMigrationStartDate =>
            new FeatureSetting(nameof(ComplianceAnalysisMigrationStartDate), "Data inicial para o pooling de migração do legado a analise cadastral", "2021-07-12");
}
```

Exemplo de consumo:
```csharp
[ApiController]
[Route("api/sample")]
public class SampleController : Controller
{
    private readonly IFeatureFlagService _featureFlagService;
    private readonly IFeatureSettingService _featureSettingService;

    public SampleController(
        IFeatureFlagService featureFlagService,
        IFeatureSettingService featureSettingService
    )
    {
        _featureFlagService = featureFlagService;
        _featureSettingService = featureSettingService;
    }

    [HttpGet("feature-setting")]
    public async Task<IActionResult> FeatureSetting(CancellationToken cancellationToken)
    {
        var startDate = await _featureSettingService
            .RetrieveAsync(nameof(AppConfigurationSettings.ComplianceAnalysisMigrationStartDate), cancellationToken)
            .ConfigureAwait(false);

        return new OkObjectResult(startDate);
    }

    [HttpGet("feature-flag")]

    public async Task<IActionResult> FeatureFlag(CancellationToken cancellationToken)
    {
        var sinacorIntegration = await _featureFlagService
            .IsEnabledAsync(nameof(AppConfigurationSettings.XpSinacorIntegration), cancellationToken)
            .ConfigureAwait(false);

        return new OkObjectResult(sinacorIntegration);
    }
}
```


## AppConfiguration.ManagerUI


O projeto AppConfiguration.ManagerUI é uma interface gráfica utilizada para vizualizar e gerenciar as FeatureFlags/Settings do projeto.

Para integrar AppConfiguration.ManagerUI ao seu projeto, basta adicionar o middlewares responsável por mapear as rotas de gerenciamento ** MapAppConfigurationUi **

```csharp
using AppConfiguration.Manager;
using AppConfiguration.ManagerUI.Extensions;

public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseEndpoints(endpoints =>
            {
                endpoints.MapAppConfigurationUi(options =>
                {
                    options.ApiNamespace = env.IsDevelopment() ? string.Empty : "app-configuration-manager";
                });
            });
    }
}
```