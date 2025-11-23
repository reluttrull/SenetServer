using SenetServer.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace SenetServer.Matchmaking
{
    public class MatchmakingService : BackgroundService
    {
        private readonly IMatchmakingQueue _matchmakingQueue;
        private readonly ILogger<MatchmakingService> _logger;

        public MatchmakingService(IMatchmakingQueue matchmakingQueue, ILogger<MatchmakingService> logger)
        {
            _matchmakingQueue = matchmakingQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Matchmaking Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var first = await _matchmakingQueue.DequeueAsync(stoppingToken);
                    _logger.LogInformation("Dequeued first match request for user {UserId} (queued at {TimeAdded})", first.UserId, first.TimeAdded);

                    var second = await _matchmakingQueue.DequeueAsync(stoppingToken);
                    _logger.LogInformation("Dequeued second match request for user {UserId} (queued at {TimeAdded})", second.UserId, second.TimeAdded);

                    await ProcessMatchmakingPairAsync(first, second, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing matchmaking request.");
                }
            }
        }

        private Task ProcessMatchmakingPairAsync(MatchRequest a, MatchRequest b, CancellationToken stoppingToken)
        {
            _logger.LogInformation("Paired users {A} and {B}", a.UserId, b.UserId);

            return Task.CompletedTask;
        }
    }
}
