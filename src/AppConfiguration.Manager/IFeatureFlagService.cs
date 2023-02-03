using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppConfiguration.Manager
{
    public interface IFeatureFlagService
    {
        Task<IEnumerable<FeatureFlag>> RetrieveAsync(CancellationToken cancellationToken);

        Task<FeatureFlag> RetrieveAsync(string id, CancellationToken cancellationToken);

        Task<bool> IsEnabledAsync(string id, CancellationToken cancellationToken);

        Task<FeatureFlag> ToggleAsync(string id, CancellationToken cancellationToken);

        IFeatureFlagService SetUser(User user);

        Task RemoveAsync(string id, CancellationToken cancellationToken);

        Task<FeatureFlag> ChangeEnabledAsync(string id, bool enabled, CancellationToken cancellationToken);
    }
}