import Link from 'next/link';
import { Table, TableBody, TableHead, TableRow, TableCell, makeStyles } from "@material-ui/core";
import { ActivityListProps } from "./activity.models";

const useStyles = makeStyles((theme) => ({
  activityIdColumnHead: {
    width: "30%"
  },
  nameColumnHead: {
    width: "40%",
    fontWeight: "bold"
  },
  freeSeatsColumnHead: {
    width: "10%"
  },
  totalSeatsColumnHead: {
    width: "10%"
  },
  ticketTypesColumnHead: {
    width: "10%"
  }
}));

export default function ActivityList({ activities }: ActivityListProps) {
  const classes = useStyles();
  return (<Table size="small">
    <TableHead>
      <TableRow>
        <TableCell className={classes.activityIdColumnHead}>ActivityId</TableCell>
        <TableCell className={classes.nameColumnHead}>Name</TableCell>
        <TableCell className={classes.freeSeatsColumnHead}>Free Seats</TableCell>
        <TableCell className={classes.totalSeatsColumnHead}>Total Seats</TableCell>
        <TableCell className={classes.ticketTypesColumnHead}>Ticket Types</TableCell>
      </TableRow>
    </TableHead>
    <TableBody>
      {activities.map(row => (
        <Link key={row.activityId} href={`/activities/${row.activityId}`}>
          <TableRow hover={true}>
            <TableCell>{row.activityId}</TableCell>
            <TableCell>{row.name}</TableCell>
            <TableCell>{row.freeSeats}</TableCell>
            <TableCell>{row.totalSeats}</TableCell>
            <TableCell>{row.ticketTypes}</TableCell>
          </TableRow>
        </Link>
      ))}
    </TableBody>

  </Table>);
}

