using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperShelf.TokenBucket.Manager
{
    public class TokenBucketManager : ITokenBucketManager
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        public int InvocationCount { get; private set; }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="bucketSize"></param>
        /// <param name="frequency"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync<T>(Func<Task<T>> func, int bucketSize, TimeSpan frequency, CancellationToken cancellationToken = new CancellationToken())
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

            while (!cancellationToken.IsCancellationRequested)
            {
                await func.Invoke();
                InvocationCount++;

                if (InvocationCount % bucketSize != 0)
                {
                    continue;
                }

                await ManageExecutionsPerUnitTime(frequency);
            }
        }

        /// <summary>
        /// If the stopwatch elapsed time is less then the total frequency count then we need to wait for the difference
        /// If the stopwatch time is less than the elapse time and the frequency limit as been reached then we just need to reset the stopwatch
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        protected async Task ManageExecutionsPerUnitTime(TimeSpan frequency)
        {
            if (_stopwatch.ElapsedMilliseconds < frequency.TotalMilliseconds)
            {
                var tempDelay = frequency.TotalMilliseconds - _stopwatch.ElapsedMilliseconds;
                await Task.Delay(Convert.ToInt32(tempDelay));
            }
            else
            {
                _stopwatch.Reset();
                _stopwatch.Start();
            }
        }
    }
}