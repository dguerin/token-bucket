using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperShelf.TokenBucket.Manager
{
    public class TokenBucketManagerManager : ITokenBucketManager
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="bucketSize"></param>
        /// <param name="frequency"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Start<T>(Func<T> func, int bucketSize, TimeSpan frequency, CancellationToken cancellationToken = new CancellationToken())
        {
            if (bucketSize == default)
            {
                throw new ArgumentNullException(nameof(bucketSize), "cannot be default value of 0");
            }

            if (frequency == default)
            {
                frequency = TimeSpan.FromSeconds(1d);
            }

            if (frequency.TotalMilliseconds < 1000)
            {
                throw new ArgumentException("frequency should not be less than 1 second", nameof(frequency));
            }

            var invokeCount = bucketSize;
            while (cancellationToken.IsCancellationRequested)
            {
                if (invokeCount == 0)
                {
                    if (_stopwatch.ElapsedMilliseconds < frequency.TotalMilliseconds)
                    {
                        var tempDelay = frequency.TotalMilliseconds - _stopwatch.ElapsedMilliseconds;
                        await Task.Delay(Convert.ToInt32(tempDelay), cancellationToken);
                    }
                    else
                    {
                        _stopwatch.Reset();
                    }
                    
                    invokeCount = bucketSize;
                }
                func.Invoke();
                invokeCount--;
            }
        }
    }
}