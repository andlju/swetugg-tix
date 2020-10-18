Swetugg Tix
===========

Swetugg Tix is an event-sourced system for managing event tickets. Just getting started though, not much to see here yet.

Activities
----------

An Activity is the root of any Tix event. An Activity has a total Seat limit that can be increased or decreased. An Activity can also have one or more TicketTypes. A TicketType can be either limited (only allow a specific number of seat reservations) or unlimited (can be used as long as there are seats left on the activity).

Tickets
-------
A Order is used to reserve a seat at an Activity. When a order is Created, an attempt will be made to reserve a seat. If the reservation succeeds, the order can be Confirmed. 