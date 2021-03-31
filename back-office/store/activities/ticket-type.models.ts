export type TicketType = {
  activityId: string;
  ownerId: string;
  ticketTypeId: string;
  revision: number;
  name: string;
  reserved: number;
  limit?: number;
};
