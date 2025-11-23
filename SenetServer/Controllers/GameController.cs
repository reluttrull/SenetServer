using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders.Physical;
using SenetServer.Matchmaking;
using SenetServer.Model;
using SenetServer.Shared;
using System;

namespace SenetServer.Controllers
{
    [ApiController]
    [Route("game")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMatchmakingQueue _matchmakingQueue;

        public GameController(
            ILogger<GameController> logger,
            IHubContext<NotificationHub> hubContext,
            IMatchmakingQueue matchmakingQueue)
        {
            _logger = logger;
            _hubContext = hubContext;
            _matchmakingQueue = matchmakingQueue;
        }

        [HttpGet]
        [Route("requestjoingame")]
        public async Task<IActionResult> RequestJoinGame([FromQuery] string? userName) // eventually generate username randomly in server
        {
            var userId = UserIdentity.GetOrCreateUserId(HttpContext);

            var request = new MatchRequest
            {
                UserId = userId,
                UserName = userName ?? $"Anonymous {new Random().Next(10000)}",
                TimeAdded = DateTime.UtcNow
            };

            await _matchmakingQueue.EnqueueAsync(request);
            _logger.LogInformation("Enqueued match request for user {UserId}", userId);

            return Ok(new { UserId = userId, UserName = userName });
        }
    }
}