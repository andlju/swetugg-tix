
export interface Activity {
  activityId: string,
  name: string,
  freeSeats: number,
  totalSeats: number,
  ticketTypes: number
}

export interface ActivitiesState {
  activities: Activity[],
  activity?: Activity
}