using System.Threading.Tasks;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.Services.Contracts
{
    public interface IUserApiService
    {
        Task<NewUserResponse> CreateNewAADUser();
    }
}
