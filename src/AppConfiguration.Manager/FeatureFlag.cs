using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppConfiguration.Manager
{
    public class FeatureFlag : IFeatureFlag
    {
        public string Id { get; set; }

        public string Description { get; }

        public bool Enabled { get; private set; }

        public FeatureFlag(string id, string description, bool enabled)
        {
            Id = id;
            Description = description;
            Enabled = enabled;
        }

        public static FeatureFlag Empty => new FeatureFlag(string.Empty, string.Empty, false);

        public FeatureFlag ChangeEnabled(bool enabled)
        {
            Enabled = enabled;

            return this;
        }

        public FeatureFlag AddPrefixToId(string prefix)
        {
            prefix = string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}:";
            
            Id = $"{prefix}{Id}";

            return this;
        }

        public void Toggle() => ChangeEnabled(!Enabled);

        public static FeatureFlag GetByAppConfigurationSettings(string id)
        {
            return GetByAppConfigurationSettings()
                .FirstOrDefault(f => id == f.Id);
        }

        public static IEnumerable<FeatureFlag> GetByAppConfigurationSettings()
        {
            var settings = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => typeof(IAppConfigurationSettings).IsAssignableFrom(x))
                ?.GetProperties() ?? Array.Empty<PropertyInfo>();

            return settings
                .Where(p => p.PropertyType.Name == nameof(FeatureFlag))
                .Select(p => (FeatureFlag)p.GetValue(settings));
        }
    }
}
