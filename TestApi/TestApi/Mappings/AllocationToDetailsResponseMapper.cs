using TestApi.Contract.Responses;
using TestApi.Domain;

namespace TestApi.Mappings
{
    public static class AllocationToDetailsResponseMapper
    {
        public static AllocationDetailsResponse MapToResponse(Allocation allocation)
        {
            return new AllocationDetailsResponse
            {
                Id = allocation.Id,
                UserId = allocation.UserId,
                Username = allocation.Username,
                Allocated = allocation.Allocated,
                ExpiresAt = allocation.ExpiresAt
            };
        }
    }
}