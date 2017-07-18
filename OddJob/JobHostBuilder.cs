using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace OddJob
{
    public sealed class JobHostBuilder
    {
        private readonly IList<Func<IJob>> processes = new List<Func<IJob>>();

        private ILoggerFactory loggerFactory;

        public JobHostBuilder Add<T>()
            where T : IJob, new()
        {
            this.Add(() => new T());
            return this;
        }

        public JobHostBuilder Add(Func<IJob> factory)
        {
            this.processes.Add(factory);
            return this;
        }

        public JobHostBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            return this;
        }

        public IJobHost Build()
        {
            return new JobHost(this.processes.Select(x => x()).ToArray(), this.loggerFactory);
        }
    }
}