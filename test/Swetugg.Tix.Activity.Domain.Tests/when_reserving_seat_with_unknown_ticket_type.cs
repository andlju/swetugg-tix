using System;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_reserving_seat_with_unknown_ticket_type : with_activity
    {
        public when_reserving_seat_with_unknown_ticket_type(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();

        protected override void Setup()
        {
            Given
                .Activity(ActivityId)
                .WithSeats(20);
        }

        protected override object When()
        {
            return new ReserveSeat()
            {
                ActivityId = ActivityId,
                TicketTypeId = Guid.NewGuid(),
                Reference = "MyRef"
            };
        }

        [Fact]
        public void then_ActivityException_is_thrown()
        {
            Assert.IsType<ActivityException>(ThrownException);
        }

        [Fact]
        public void then_errorcode_is_UnknownTicketType()
        {
            var activityException = ThrownException as ActivityException;
            Assert.NotNull(activityException);
            Assert.Equal("UnknownTicketType", activityException.ErrorCode);
        }
    }
}