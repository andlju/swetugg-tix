
export type TicketType = {
  activityId: string,
  ticketTypeId: string,
  name: string
}

export interface TicketTypesView {
  revision: number,
  ticketTypes: TicketType[]
}
