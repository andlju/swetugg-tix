import { TicketType } from "./ticket-type.models";

export type Activity = {
  activityId: string,
  revision: number,
  name: string,
  freeSeats: number,
  totalSeats: number,
  ticketTypes: TicketType[]
}

