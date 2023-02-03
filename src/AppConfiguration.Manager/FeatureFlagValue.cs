namespace AppConfiguration.Manager
{
    public class FeatureFlagValue
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public object Conditions { get; } = new
        {
            client_filters = new object[] { }
        };

        public FeatureFlagValue(string id, string description, bool enabled)
        {
            Id = id;
            Description = description;
            Enabled = enabled;
        }
    }
}