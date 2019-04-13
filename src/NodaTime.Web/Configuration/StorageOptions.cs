﻿// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Google.Api.Gax;
using Microsoft.Extensions.DependencyInjection;
using NodaTime.Web.Services;

namespace NodaTime.Web.Configuration
{
    /// <summary>
    /// Options for how NodaTime releases, IANA releases and benchmarks
    /// are loaded.
    /// </summary>
    public class StorageOptions
    {
        /// <summary>
        /// The storage bucket to use. This is either a GCS bucket, or "local:{directory}"
        /// for a local file system directory.
        /// </summary>
        public string? Bucket { get; set; }

        /// <summary>
        /// The number of benchmark runs to load.
        /// </summary>
        public int? BenchmarkLimit { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            GaxPreconditions.CheckState(Bucket != null, "Bucket must be configured");
            var repository = new GoogleCloudStorageRepository(Bucket!);

            services.AddSingleton<IStorageRepository>(repository);
            services.AddSingleton<IReleaseRepository, GoogleStorageReleaseRepository>();
            services.AddSingleton<ITzdbRepository, GoogleStorageTzdbRepository>();
            services.AddSingleton<IBenchmarkRepository, GoogleStorageBenchmarkRepository>(serviceProvider =>
                ActivatorUtilities.CreateInstance<GoogleStorageBenchmarkRepository>(serviceProvider, BenchmarkLimit ?? int.MaxValue));
        }
    }
}