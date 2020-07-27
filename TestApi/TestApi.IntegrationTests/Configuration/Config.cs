using TestApi.Common.Configuration;

namespace TestApi.IntegrationTests.Configuration
{
    public class Config
    {
        public AzureAdConfiguration AzureAdConfiguration { get; set; }
        public DbConfig DbConnection { get; set; }
        public string UsernameStem { get; set; }
        public ServicesConfiguration VhServices { get; set; }
    }
}
