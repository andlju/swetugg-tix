using Swetugg.Tix.Activity.Commands;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Activity.Domain.Tests
{
    public class when_reserving_seat_on_unlimited_ticket_type_on_full_activity : with_activity
    {
        public when_reserving_seat_on_unlimited_ticket_type_on_full_activity(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected override void Setup()
        {
            var activity = Given
                .Activity(ActivityId);

            activity
                .WithSeats(20)
                .WithTicketType(TicketTypeId)
                .WithRemovedTicketTypeLimit(TicketTypeId);
            for (int i = 0; i < 20; i++)
            {
                activity
                    .WithReservedSeat(TicketTypeId, i.ToString());
            }

        }

        protected override object When()
        {
            return new ReserveSeat()
            {
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId,
                Reference = "MyRef"
            };
        }

        [Fact]
        public void then_nothing_is_committed()
        {
            Assert.Null(Commits.FirstOrDefault());
        }

        [Fact]
        public void then_exception_is_thrown()
        {
            Assert.IsType<ActivityException>(this.ThrownException);
        }
    }

    public class when_reserving_seat_on_limited_ticket_type_on_full_activity : with_activity
    {
        public when_reserving_seat_on_limited_ticket_type_on_full_activity(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();
        protected Guid TicketTypeId2 = Guid.NewGuid();

        protected override void Setup()
        {
            var activity = Given
                .Activity(ActivityId);

            activity
                .WithSeats(20)
                .WithTicketType(TicketTypeId)
                .WithIncreasedTicketTypeLimit(TicketTypeId, 15)
                .WithTicketType(TicketTypeId2)
                .WithIncreasedTicketTypeLimit(TicketTypeId2, 15);

            for (int i = 0; i < 15; i++)
            {
                activity
                    .WithReservedSeat(TicketTypeId, i.ToString());
            }

            for (int i = 0; i < 5; i++)
            {
                activity
                    .WithReservedSeat(TicketTypeId2, i.ToString());
            }

        }

        protected override object When()
        {
            return new ReserveSeat()
            {
                ActivityId = ActivityId,
                TicketTypeId = TicketTypeId2,
                Reference = "MyRef"
            };
        }

        [Fact]
        public void then_nothing_is_committed()
        {
            Assert.Null(Commits.FirstOrDefault());
        }

        [Fact]
        public void then_exception_is_thrown()
        {
            Assert.IsType<ActivityException>(this.ThrownException);
        }
    }

}