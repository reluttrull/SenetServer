using System;
using System.Collections.Generic;
using System.Text;

namespace SenetServer.Contracts.Responses
{
    public class UserInfoResponse
    {
        public required string UserId { get; init; }
        public required string UserName { get; init; }
    }
}
