using System;
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
        /// <param name="frequencyPerSecond">Represents the amount of invocations of the function per second</param>
        /// <param name="operationDurationInMilliseconds">Represents the fake operation duration</param>
        /// <param name="cancelRequestedInMilliseconds">Represents how long the test will run for</param>
        /// <param name="expectedInvocation">Represents the expected number of invocations of the fake function</param>
        [Theory]
        [InlineData(10, 2, 1000, 2000, 10)]
        [InlineData(10, 2, 100, 2000, 10)]
        [InlineData(100, 10, 10, 2000, 100)]
        [InlineData(100, 20, 10, 2000, 200)]
        public void BasicExecutionTest(int bucketSize, int frequencyPerSecond, int operationDurationInMilliseconds, int cancelRequestedInMilliseconds, int expectedInvocation)
        {
            var cancellationToken = new CancellationTokenSource();

            var manager = new TokenBucketManager();
            var task1 =  manager.StartAsync(async () =>
            {
                await Task.Delay(operationDurationInMilliseconds, CancellationToken.None);
                return new object();
            }, 
            bucketSize, 
            TimeSpan.FromSeconds(frequencyPerSecond), cancellationToken.Token);

            var task2 = Task.Run(async () =>
            {
                await Task.Delay(cancelRequestedInMilliseconds, CancellationToken.None);
                cancellationToken.Cancel();
            }, CancellationToken.None);
            
            Task.WaitAll(task1, task2);

            Assert.True(expectedInvocation >= manager.InvocationCount);
        }
    }
}
