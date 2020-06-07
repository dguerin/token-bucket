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
        /// <param name="expectedInvocationRangeLower">Represents the expected lower limit of invocation of the fake function</param>
        /// <param name="expectedInvocationRangeUpper">Represents the expected upper limit of invocations of the fake function</param>
        [Theory]
        [InlineData(10, 1000, 2000, 1, 2)]
        [InlineData(10, 100, 2000, 15, 20)]
        [InlineData(10, 1, 2000, 15, 20)]
        [InlineData(100, 1, 2000, 100, 200)]
        [InlineData(100, 10, 2000, 100, 200)]
        public void BasicExecutionTest(
            int bucketSize,
            int operationDurationInMilliseconds, 
            int cancelRequestedInMilliseconds,
            int expectedInvocationRangeLower,
            int expectedInvocationRangeUpper)
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

            Assert.InRange(manager.InvocationCount, expectedInvocationRangeLower, expectedInvocationRangeUpper );
        }
    }
}
