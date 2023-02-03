using AppConfiguration.Manager;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace AppConfiguration.Api.Controllers
{
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
}
