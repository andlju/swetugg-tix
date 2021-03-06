﻿using Swetugg.Tix.Activity.Commands;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_adding_a_negative_number_of_seats_to_activity : with_activity
    {
        protected Guid ActivityId = Guid.NewGuid();

        public when_adding_a_negative_number_of_seats_to_activity(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Setup()
        {
            Given.
                Activity(ActivityId, UserId, OwnerId);
        }

        protected override object When()
        {
            return new AddSeats()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                Seats = -10
            };
        }

        [Fact]
        public void then_the_command_fails()
        {
            Assert.True(Command.HasFailed);
        }

        [Fact]
        public void then_ErrorCode_is_InvalidInput()
        {
            Assert.Equal("InvalidInput", Command.FailureCode);
        }
    }
}