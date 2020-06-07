using System.Threading;
using System.Threading.Tasks;
using DeveloperShelf.TokenBucket.Manager;
using Xunit;

namespace DeveloperShelf.TokenBucket.Tests
{
    public class TokenBucketTests
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketSize">Represents the number of operations that are allowed per unit time</param>
        /// <param name="operationDurationInMilliseconds">Represents the fake operation duration</param>
        /// <param name="cancelRequestedInMilliseconds">Represents how long the test will run for</param>
        /// <param name="expectedInvocationLimit">Represents the expected upper limit of invocations of the fake function</param>
        [Theory]
        [InlineData(10, 1000, 2000, 2)]
        [InlineData(10, 100, 2000, 20)]
        [InlineData(10, 1, 2000, 20)]
        [InlineData(100, 1, 2000, 200)]
        [InlineData(100, 10, 2000, 200)]
        [InlineData(100, 10, 5000, 500)]
        [InlineData(100, 10, 10000, 1000)]
        public void EnsureExecutionLimit(
            int bucketSize,
            int operationDurationInMilliseconds, 
            int cancelRequestedInMilliseconds,
            int expectedInvocationLimit)
        {
            var cancellationToken = new CancellationTokenSource();

            var manager = new TokenBucketManager();
            var task1 =  manager.StartAsync(async () =>
            {
                await Task.Delay(operationDurationInMilliseconds, CancellationToken.None);
                return new object();
            }, 
            bucketSize, cancellationToken.Token);

            var task2 = Task.Run(async () =>
            {
                await Task.Delay(cancelRequestedInMilliseconds, CancellationToken.None);
                cancellationToken.Cancel();
            }, CancellationToken.None);
            
            Task.WaitAll(task1, task2);

            Assert.True(manager.InvocationCount <= expectedInvocationLimit);
        }
    }
}
