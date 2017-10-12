using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OddJob.Schedules;

namespace OddJob
{
    /// <summary>
    /// Represents a builder that creates an <see cref="IJobHost"/>.
    /// </summary>
    public sealed class JobHostBuilder
    {
        private readonly IList<Func<IJob>> processes = new List<Func<IJob>>();

        private ILoggerFactory loggerFactory = NullLoggerFactory.Instance;

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
        /// Creates an instance of <typeparamref name="T"/> that will be managed
        /// by the <see cref="IJobHost"/> built by <see cref="JobHostBuilder"/> and run on a <paramref name="schedule"/>.
        /// </summary>
        /// <typeparam name="T">An <see cref="IJob"/> to add to the built <see cref="JobHost"/>.</typeparam>
        /// <param name="schedule">The <see cref="ISchedule"/> on which the <typeparamref name="T"/> will run.</param>
        /// <returns>A <see cref="JobHostBuilder"/>.</returns>
        public JobHostBuilder Add<T>(ISchedule schedule)
            where T : IJob, new()
        {
            this.Add(() => new T(), schedule);
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
        /// Add a <see cref="IJob"/> instance that will be managed by
        /// a <see cref="IJobHost"/> built by <see cref="JobHostBuilder"/>.
        /// </summary>
        /// <param name="job">A <see cref="IJob"/>.</param>
        /// <param name="schedule">The <see cref="ISchedule"/> on which the <paramref name="job"/> will run.</param>
        /// <returns>A <see cref="JobHostBuilder"/>.</returns>
        public JobHostBuilder Add(IJob job, ISchedule schedule)
        {
            this.Add(() => job, schedule);
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
        /// Add a callback to create a <see cref="IJob"/> that will be managed by
        /// a <see cref="IJobHost"/> built by <see cref="JobHostBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IJob"/> to add.</typeparam>
        /// <param name="factory">A function that creates an <see cref="IJob"/>.</param>
        /// <param name="schedule">The <see cref="ISchedule"/> on which the <paramref name="factory"/> <see cref="IJob"/> will run.</param>
        /// <returns>A <see cref="JobHostBuilder"/>.</returns>
        public JobHostBuilder Add<T>(Func<T> factory, ISchedule schedule)
            where T : IJob
        {
            this.processes.Add(() => new Jobs.ScheduledJob<T>(factory, schedule, this.loggerFactory, Clock.DefaultClock));

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
            return new JobHost(this.processes.Select(x => x()).ToArray(), this.loggerFactory);
        }
    }
}
