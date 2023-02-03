using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppConfiguration.Manager
{
    public interface IFeatureSettingService
    {
        Task<IEnumerable<FeatureSetting>> RetrieveAsync(CancellationToken cancellationToken);
        
        Task<FeatureSetting> RetrieveAsync(string id, CancellationToken cancellationToken);

        Task<FeatureSetting> ChangeValueAsync(string id, string value, CancellationToken cancellationToken);

        Task RemoveAsync(string id, CancellationToken cancellationToken);

        IFeatureSettingService SetUser(User user);
    }
}