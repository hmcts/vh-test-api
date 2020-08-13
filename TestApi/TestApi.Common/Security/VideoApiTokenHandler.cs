using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TestApi.Common.Configuration;

namespace TestApi.Common.Security
{
    public class VideoApiTokenHandler : BaseServiceTokenHandler
    {
        public VideoApiTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
            IOptions<ServicesConfiguration> servicesConfiguration, IMemoryCache memoryCache,
            ITokenProvider tokenProvider) : base(azureAdConfiguration, servicesConfiguration, memoryCache,
            tokenProvider)
        {
        }

        protected override string TokenCacheKey => "VideoApiServiceToken";
        protected override string ClientResource => ServicesConfiguration.VideoApiResourceId;
    }
}