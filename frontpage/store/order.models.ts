
export type OrderTicket = {
  ticketId: string;
  orderId: string;
  ticketTypeId: string;
  ticketReference: string | null;
  statusCode: string;
};

export type Order = {
  orderId: string;
  activityId: string;
  revision: number;
  tickets: OrderTicket[];
};
