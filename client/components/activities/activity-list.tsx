import { ComponentProps } from "react";
import { GetServerSideProps } from "next";
import { Table, TableBody, TableHead, TableRow, TableCell } from "@material-ui/core";

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


export default function ActivityList({ activities }: ActivityListProps) {

  return (<Table>
    <TableHead>
      <TableRow>
        <TableCell>ActivityId</TableCell>
        <TableCell>Name</TableCell>
        <TableCell>Free Seats</TableCell>
        <TableCell>Total Seats</TableCell>
        <TableCell>Ticket Types</TableCell>
      </TableRow>
    </TableHead>
    <TableBody>
      {activities.map(a => (
        <TableRow key={a.activityId}>
          <TableCell>{a.activityId}</TableCell>
          <TableCell>{a.name}</TableCell>
          <TableCell>{a.freeSeats}</TableCell>
          <TableCell>{a.totalSeats}</TableCell>
          <TableCell>{a.ticketTypes}</TableCell>
        </TableRow>
      ))}
    </TableBody>

  </Table>);
}

