
export type Activity = {
  activityId: string,
  revision: number,
  name: string,
  freeSeats: number,
  totalSeats: number,
  ticketTypes: number
}

export type ActivityListProps = {
  activities: Activity[]
}

export type ActivityDetailsProps = {
  activity: Activity,
  refreshActivityRevision: (revision: number) => void
}
