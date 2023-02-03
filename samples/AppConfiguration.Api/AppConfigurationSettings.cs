using AppConfiguration.Manager;

namespace AppConfiguration.Api
{
    public sealed class AppConfigurationSettings : IAppConfigurationSettings
    {
        public static FeatureFlag XpSinacorIntegration =>
            new FeatureFlag(nameof(XpSinacorIntegration), "Feature flag responsavel por habilitar a integração como o Sinacor da XP", false);

        public static FeatureFlag RicoSinacorIntegration =>
            new FeatureFlag(nameof(RicoSinacorIntegration), "Feature flag responsavel por habilitar a integração como o Sinacor da Rico", false);

        public static FeatureFlag RicoComplianceAnalysisMigration =>
            new FeatureFlag(nameof(RicoComplianceAnalysisMigration), "Feature flag responsavel por habilitar a migração das analises cadastrais da Rico", false);
        
        public static FeatureSetting ComercialAddressBackgroundServiceLimit =>
            new FeatureSetting(nameof(ComercialAddressBackgroundServiceLimit), "Limite para o pooling de sincronização de endereço comercial", string.Empty);

        public static FeatureSetting ComplianceAnalysisMigrationLimitDate =>
            new FeatureSetting(nameof(ComplianceAnalysisMigrationLimitDate), "Data limite para o pooling de migração do legado a analise cadastral", string.Empty);

        public static FeatureSetting ComplianceAnalysisMigrationStartDate =>
            new FeatureSetting(nameof(ComplianceAnalysisMigrationStartDate), "Data inicial para o pooling de migração do legado a analise cadastral", string.Empty);
    }
}
