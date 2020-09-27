
export type Activity = {
  activityId: string,
  name: string,
  freeSeats: number,
  totalSeats: number,
  ticketTypes: number
}

export type ActivityListProps = {
  activities: Activity[]
}

export type ActivityDetailsProps = {
  activity: Activity
}
