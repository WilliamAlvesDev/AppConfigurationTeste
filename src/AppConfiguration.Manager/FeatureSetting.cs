using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppConfiguration.Manager
{
    public class FeatureSetting : IFeatureSetting
    {
        public string Id { get; private set; }

        public string Description { get; }

        public string Value { get; private set; }

        public FeatureSetting(string id, string description, string value)
        {
            Id = id;
            Description = description;
            Value = value;
        }

        public FeatureSetting ChangeValue(string value)
        {
            Value = value;

            return this;
        }

        public FeatureSetting AddPrefixToId(string prefix)
        {
            prefix = string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}:";

            if (Id.Contains(prefix))
            {
                return this;
            }

            Id = $"{prefix}{Id}";

            return this;
        }

        public static FeatureSetting Empty => new FeatureSetting(default, default, default);

        public static FeatureSetting GetByAppConfigurationSettings(string id)
        {
            return GetByAppConfigurationSettings()
                .FirstOrDefault(f => id == f.Id);
        }

        public static IEnumerable<FeatureSetting> GetByAppConfigurationSettings()
        {
            var settings = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => typeof(IAppConfigurationSettings).IsAssignableFrom(x))
                ?.GetProperties() ?? Array.Empty<PropertyInfo>();

            return settings
                .Where(p => p.PropertyType.Name == nameof(FeatureSetting))
                .Select(p => (FeatureSetting)p.GetValue(settings));
        }
    }
}