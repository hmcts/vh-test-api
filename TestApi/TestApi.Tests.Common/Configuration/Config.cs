using TestApi.Common.Configuration;

namespace TestApi.Tests.Common.Configuration
{
    public class Config
    {
        public AzureAdConfiguration AzureAdConfiguration { get; set; }
        public DbConfig DbConnection { get; set; }
        public ServicesConfiguration Services { get; set; }
        public string UsernameStem { get; set; }
        public string EjudUsernameStem { get; set; }
        public WowzaConfiguration Wowza { get; set; }
    }
}