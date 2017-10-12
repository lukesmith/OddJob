using System;

namespace OddJob
{
#pragma warning disable SA1600 // Elements should be documented
    internal interface IClock
    {
        DateTime UtcNow { get; }
    }
#pragma warning restore SA1600 // Elements should be documented
}
