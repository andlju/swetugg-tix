using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonDomain;
using CommonDomain.Core;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Ticket.Events;

namespace Swetugg.Tix.Process
{
    public class TicketConfirmationSaga : SagaBase<object>
    {
        private Guid _activityId;
        private Guid _ticketTypeId;
        private Guid? _couponId;

        public TicketConfirmationSaga()
        {
            Register<TicketCreated>(Handle);
        }

        public void Handle(TicketCreated evt)
        {
            _activityId = evt.AggregateId;
            _ticketTypeId = evt.TicketTypeId;
            _couponId = evt.CouponId;

            // Reserve a seat
            Dispatch(new ReserveSeat()
            {
                CommandId = Guid.NewGuid(),
                ActivityId = _activityId,
                TicketTypeId = _ticketTypeId,
                CouponId = _couponId
            });
        }
    }
}
