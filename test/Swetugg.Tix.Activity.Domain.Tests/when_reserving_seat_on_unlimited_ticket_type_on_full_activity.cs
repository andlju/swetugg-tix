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
        public void then_the_command_fails()
        {
            Assert.True(Command.HasFailed);
        }
    }

}