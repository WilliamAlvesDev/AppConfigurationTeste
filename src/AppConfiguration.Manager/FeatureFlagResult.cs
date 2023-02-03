using Ardalis.SmartEnum.SystemTextJson;
using Newtonsoft.Json;

namespace AppConfiguration.Manager
{
    public readonly struct FeatureFlagResult
    {
        public FeatureFlagResult(FeatureFlag value)
        {
            Value = value;

            Status = value.Enabled ? FeatureFlagStatus.Actived : FeatureFlagStatus.Inactived;
        }

        [JsonConverter(typeof(SmartEnumNameConverter<FeatureFlagStatus, int>))]
        public FeatureFlagStatus Status { get; }

        public FeatureFlag Value { get; }
    }
}
