import { TicketType } from "./ticket-type.models";

export type Activity = {
  activityId: string,
  createdByUserId: string,
  revision: number,
  name: string,
  freeSeats: number,
  totalSeats: number,
  ticketTypes: TicketType[]
}
