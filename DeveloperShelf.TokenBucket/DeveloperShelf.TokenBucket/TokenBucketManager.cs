using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperShelf.TokenBucket.Manager
{
    public class TokenBucketManager : ITokenBucketManager
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly TimeSpan _frequency = TimeSpan.FromSeconds(1d);

        public int InvocationCount { get; private set; }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="bucketSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync<T>(Func<Task<T>> func, int bucketSize, CancellationToken cancellationToken = new CancellationToken())
        {
            if (bucketSize == default)
            {
                throw new ArgumentNullException(nameof(bucketSize), "cannot be default value of 0");
            }

            _stopwatch.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                await func.Invoke();
                InvocationCount++;

                if (InvocationCount % bucketSize != 0)
                {
                    continue;
                }

                if (_frequency.TotalMilliseconds >= _stopwatch.ElapsedMilliseconds)
                {
                    var tempDelay = _frequency.TotalMilliseconds - _stopwatch.ElapsedMilliseconds;
                    await Task.Delay(Convert.ToInt32(tempDelay), CancellationToken.None);
                }

                _stopwatch.Reset();
                _stopwatch.Start();
            }
        }
    }
}