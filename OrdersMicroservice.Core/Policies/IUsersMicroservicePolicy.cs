using Polly;

namespace OrdersMicroservice.Core.Policies;

public interface IUsersMicroservicePolicy
{
    //IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();

    //IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy();

    IAsyncPolicy<HttpResponseMessage> GetRetryThenCircuitBreakPolicy();
}
