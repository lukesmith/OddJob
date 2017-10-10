using System;
using OddJob.Schedules;
using Xunit;

namespace OddJob.Tests.Schedules
{
    public class DailyTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(24)]
        public void HourOutsideOfAllowedRange(int hour)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Daily().At(hour, 0, 0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(60)]
        public void MinuteOutsideOfAllowedRange(int minute)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Daily().At(0, minute, 0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(60)]
        public void SecondOutsideOfAllowedRange(int second)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Daily().At(0, 0, second));
        }

        [Theory]
#pragma warning disable xUnit1010 // The value is not convertible to the method parameter type
        [InlineData("2017-01-02T03:04:00", "2017-01-03T00:00:00")]
        [InlineData("2017-01-02T00:00:00", "2017-01-03T00:00:00")]
#pragma warning restore xUnit1010 // The value is not convertible to the method parameter type
        public void EveryDay(DateTime from, DateTime expected)
        {
            var schedule = Schedule.Every().Day();

            var result = schedule.Next(from);

            Assert.Equal(expected, result);
        }

        [Theory]
#pragma warning disable xUnit1010 // The value is not convertible to the method parameter type
        [InlineData("2017-01-02T03:04:00", "2017-01-03T11:32:34")]
        [InlineData("2017-01-02T00:00:00", "2017-01-03T11:32:34")]
#pragma warning restore xUnit1010 // The value is not convertible to the method parameter type
        public void EveryDayAt(DateTime from, DateTime expected)
        {
            var schedule = Schedule.Every().Day().At(11, 32, 34);

            var result = schedule.Next(from);

            Assert.Equal(expected, result);
        }
    }
}
