using Ardalis.SmartEnum;

namespace AppConfiguration.Manager
{
    public sealed class FeatureFlagStatus: SmartEnum<FeatureFlagStatus>
    {
        private FeatureFlagStatus(string name, int value) : base(name, value)
        {
        }

        public static readonly FeatureFlagStatus Undefined = new FeatureFlagStatus(nameof(Undefined), 0);

        public static readonly FeatureFlagStatus Actived = new FeatureFlagStatus(nameof(Actived), 1);

        public static readonly FeatureFlagStatus Inactived = new FeatureFlagStatus(nameof(Inactived), 2);
    }
}
