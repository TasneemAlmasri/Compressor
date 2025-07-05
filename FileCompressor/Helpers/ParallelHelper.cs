using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FileCompressorApp.Helpers
{
    public static class ParallelHelper
    {
        public static void ParallelForEach<T>(
            IEnumerable<T> items,
            Action<T> action,
            int maxDegreeOfParallelism = 4,
            CancellationToken token = default)
        {
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = token
            };

            Parallel.ForEach(items, options, action);
        }
    }
}
