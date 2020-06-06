using System;
using System.Threading.Tasks;
using DeveloperShelf.TokenBucket.Manager;
using Xunit;

namespace DeveloperShelf.TokenBucket.Tests
{
    public class TokenBucketTests
    {
        [Theory]
        [InlineData(10, 1)]
        public async Task Test1(int bucketSize, int frequencyPerSecond)
        {
            var manager = new TokenBucketManagerManager();
            await manager.Start(async () =>
            {
                await Task.Delay(1000);
            }, bucketSize, TimeSpan.FromSeconds(frequencyPerSecond));
        }
    }
}
