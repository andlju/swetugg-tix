
export type TicketType = {
  activityId: string,
  ticketTypeId: string,
  name: string
}

export type TicketTypeListProps = {
  ticketTypes: TicketType[]
}

export type TicketTypeDetailsProps = {
  ticketType: TicketType
}
