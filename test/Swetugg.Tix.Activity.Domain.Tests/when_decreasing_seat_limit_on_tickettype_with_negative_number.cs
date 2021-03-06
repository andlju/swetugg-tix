﻿using Swetugg.Tix.Activity.Commands;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{

    public class when_decreasing_seat_limit_on_tickettype_with_negative_number : with_activity
    {
        public when_decreasing_seat_limit_on_tickettype_with_negative_number(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId, UserId, OwnerId)
                .WithSeats(20)
                .WithTicketType(TicketTypeId)
                .WithIncreasedTicketTypeLimit(TicketTypeId, 10);
        }

        protected override object When()
        {
            return new DecreaseTicketTypeLimit()
            {
                ActivityId = ActivityId,
                OwnerId = OwnerId,
                TicketTypeId = TicketTypeId,
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