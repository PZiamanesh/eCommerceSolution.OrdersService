using Microsoft.Extensions.Logging;
using Polly;

namespace OrdersMicroservice.Core.Policies;

public class UsersMicroservicePolicy : IUsersMicroservicePolicy
{
    private readonly ILogger<UsersMicroservicePolicy> logger;

    public UsersMicroservicePolicy(ILogger<UsersMicroservicePolicy> logger)
    {
        this.logger = logger;
    }

    public IAsyncPolicy<HttpResponseMessage> GetRetryThenCircuitBreakPolicy()
    {
        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(2),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    logger.LogInformation($"retry {retryAttempt} after {timespan} seconds");
                });

        var circuitBreakerPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(10),
                onBreak: (outcome, timespan) =>
                {
                    logger.LogInformation($"circuit is open for {timespan} seconds before half-open state");
                },
                onReset: () =>
                {
                    logger.LogInformation("circuit now closed");
                });


        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
    }
}
