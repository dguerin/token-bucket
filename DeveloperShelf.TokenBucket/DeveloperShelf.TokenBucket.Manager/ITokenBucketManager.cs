﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperShelf.TokenBucket.Manager
{
    public interface ITokenBucketManager
    {
        /// <summary>
        /// 
        /// </summary>
        int InvocationCount { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">The function to execute</param>
        /// <param name="bucketSize">Specify the bucket size to control flow</param>
        /// <param name="time">Defaults to 1 second intervals</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartAsync<T>(Func<Task<T>> func, int bucketSize, TimeSpan time, CancellationToken cancellationToken = new CancellationToken());
    }
}
