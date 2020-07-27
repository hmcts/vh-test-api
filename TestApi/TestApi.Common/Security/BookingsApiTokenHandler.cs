using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TestApi.Common.Configuration;

namespace TestApi.Common.Security
{
    public class BookingsApiTokenHandler : BaseServiceTokenHandler
    {
        public BookingsApiTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
            IOptions<ServicesConfiguration> servicesConfiguration, IMemoryCache memoryCache,
            ITokenProvider tokenProvider) : base(azureAdConfiguration, servicesConfiguration, memoryCache,
            tokenProvider)
        {
        }
        
        protected override string TokenCacheKey => "BookingsApiServiceToken";
        protected override string ClientResource => ServicesConfiguration.BookingsApiResourceId;
    }
}