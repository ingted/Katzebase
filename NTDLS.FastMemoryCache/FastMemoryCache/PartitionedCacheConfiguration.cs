﻿namespace NTDLS.FastMemoryCache
{
    /// <summary>
    /// Defines the configuration for a partitioned memory cache instance.
    /// </summary>
    public class PartitionedCacheConfiguration
    {
        /// <summary>
        /// The number of partitions that the memory cache should be split into.
        /// </summary>
        public int PartitionCount { get; set; } = Environment.ProcessorCount * 4;

        /// <summary>
        /// The number of seconds between attempts to sure-up the set memory limits. 0 = no scavenging.
        /// </summary>
        public int ScavengeIntervalSeconds { get; set; } = 10;

        /// <summary>
        /// The maximum size of the memory cache. The cache will attempt to keep the cache to this size. 0 = no limit.
        /// </summary>
        public long MaxMemoryBytes { get; set; } = 1024L * 1024 * 1024 * 4;

        /// <summary>
        /// Whether the cache keys are treated as case sensitive or not.
        /// </summary>
        public bool IsCaseSensitive { get; set; } = true;

        /// <summary>
        /// Whether or not the cache should track object size for memory limitations and cache ejections.
        /// </summary>
        public bool TrackObjectSize { get; set; } = true;

        /// <summary>
        /// Returns a copy of the configuration instance.
        /// </summary>
        public PartitionedCacheConfiguration Clone()
        {
            return new PartitionedCacheConfiguration()
            {
                MaxMemoryBytes = MaxMemoryBytes,
                PartitionCount = PartitionCount,
                ScavengeIntervalSeconds = ScavengeIntervalSeconds,
                IsCaseSensitive = IsCaseSensitive
            };
        }
    }
}
