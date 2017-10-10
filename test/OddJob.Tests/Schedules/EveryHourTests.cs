using System;
using OddJob.Schedules;
using Xunit;

namespace OddJob.Tests.Schedules
{
    public class EveryHourTests
    {
        [Theory]
#pragma warning disable xUnit1010 // The value is not convertible to the method parameter type
        [InlineData("2017-01-02T03:04:00", "2017-01-02T04:00:00")]
        [InlineData("2017-01-02T03:04:05", "2017-01-02T04:00:00")]
        [InlineData("2017-01-02T03:59:59", "2017-01-02T04:00:00")]
        [InlineData("2017-01-02T03:00:01", "2017-01-02T04:00:00")]
        [InlineData("2017-01-02T23:00:01", "2017-01-03T00:00:00")]
        [InlineData("2017-01-31T23:00:01", "2017-02-01T00:00:00")]
#pragma warning restore xUnit1010 // The value is not convertible to the method parameter type
        public void Every(DateTime from, DateTime expected)
        {
            var schedule = Schedule.Every().Hour();

            var result = schedule.Next(from);

            Assert.Equal(expected, result);
        }
    }
}
