using TestApi.Contract.Responses;
using TestApi.DAL.Helpers;

namespace TestApi.Mappings
{
    public static class NumberToResponseMapper
    {
        public static IteratedUserNumberResponse MapToResponse(Integer integer)
        {
            return new IteratedUserNumberResponse
            {
                Number = integer
            };
        }
    }
}