using SenetServer.Model;
using SenetServer.Contracts.Responses;

namespace SenetServer.Mapping
{
    public static class ContractMapping
    {
        public static UserInfoResponse MapToResponse(this UserInfo userInfo)
        {
            return new UserInfoResponse()
            { 
                UserId = userInfo.UserId,
                UserName = userInfo.UserName
            };
        }
    }
}
