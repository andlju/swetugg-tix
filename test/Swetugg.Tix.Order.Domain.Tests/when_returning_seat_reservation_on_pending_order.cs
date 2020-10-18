﻿using Swetugg.Tix.Tests.Helpers;
using Swetugg.Tix.Order.Commands;
using Swetugg.Tix.Order.Events;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Swetugg.Tix.Order.Domain.Tests
{
    public class when_returning_seat_reservation_on_pending_order : with_order
    {
        public when_returning_seat_reservation_on_pending_order(ITestOutputHelper output) : base(output)
        {
        }

        protected Guid OrderId = Guid.NewGuid();
        protected Guid ActivityId = Guid.NewGuid();
        protected Guid TicketTypeId = Guid.NewGuid();

        protected string TicketReference = Guid.NewGuid().ToString();

        protected override void Setup()
        {
            Given.Order(OrderId, ActivityId)
                .WithAddedTicket(TicketTypeId)
                .WithReservedSeat(TicketTypeId, TicketReference);
        }

        protected override object When()
        {
            return new ConfirmReturnedSeat() { OrderId = OrderId, TicketTypeId = TicketTypeId, TicketReference = TicketReference };
        }

        [Fact]
        public void then_SeatReturned_event_is_raised()
        {
            Assert.True(Commits.First().HasEvent<SeatReturned>());
        }

        [Fact]
        public void then_TicketReference_is_correct()
        {
            Assert.Equal(TicketReference, Commits.First().GetEvent<SeatReturned>().TicketReference);
        }

        [Fact]
        public void then_TicketTypeId_is_correct()
        {
            Assert.Equal(TicketTypeId, Commits.First().GetEvent<SeatReturned>().TicketTypeId);
        }

    }
}