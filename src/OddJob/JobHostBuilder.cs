using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace OddJob
{
    /// <summary>
    /// Represents a builder that creates an <see cref="IJobHost"/>.
    /// </summary>
    public sealed class JobHostBuilder
    {
        private readonly IList<Func<IJob>> processes = new List<Func<IJob>>();

        private ILoggerFactory loggerFactory;

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> that will be managed
        /// by the <see cref="IJobHost"/> built by <see cref="JobHostBuilder"/>.
        /// </summary>
        /// <typeparam name="T">An <see cref="IJob"/> to add to the built <see cref="JobHost"/>.</typeparam>
        /// <returns>A <see cref="JobHostBuilder"/>.</returns>
        public JobHostBuilder Add<T>()
            where T : IJob, new()
        {
            this.Add(() => new T());
            return this;
        }

        /// <summary>
        /// Add a <see cref="IJob"/> instance that will be managed by
        /// a <see cref="IJobHost"/> built by <see cref="JobHostBuilder"/>.
        /// </summary>
        /// <param name="job">A <see cref="IJob"/>.</param>
        /// <returns>A <see cref="JobHostBuilder"/>.</returns>
        public JobHostBuilder Add(IJob job)
        {
            this.processes.Add(() => job);
            return this;
        }

        /// <summary>
        /// Add a callback to create a <see cref="IJob"/> that will be managed by
        /// a <see cref="IJobHost"/> built by <see cref="JobHostBuilder"/>.
        /// </summary>
        /// <param name="factory">A function that creates an <see cref="IJob"/>.</param>
        /// <returns>A <see cref="JobHostBuilder"/>.</returns>
        public JobHostBuilder Add(Func<IJob> factory)
        {
            this.processes.Add(factory);
            return this;
        }

        /// <summary>
        /// Defines the <see cref="ILoggerFactory"/> to use when building.
        /// </summary>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory"/>.</param>
        /// <returns>A <see cref="JobHostBuilder"/>.</returns>
        public JobHostBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            return this;
        }

        /// <summary>
        /// Builds a <see cref="IJobHost"/> based on the build configuration.
        /// </summary>
        /// <returns>A <see cref="JobHostBuilder"/>.</returns>
        public IJobHost Build()
        {
            return new JobHost(this.processes.Select(x => x()).ToArray(), this.loggerFactory ?? new NullLoggerFactory());
        }
    }
}
