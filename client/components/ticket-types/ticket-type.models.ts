
export type TicketType = {
  activityId: string,
  ticketTypeId: string,
  revision: number,
  name: string,
  reserved: number,
  limit?: number,
}

export interface TicketTypesView {
  revision: number,
  ticketTypes: TicketType[],
}
