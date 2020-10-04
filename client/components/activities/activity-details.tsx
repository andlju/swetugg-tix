import { makeStyles, Typography } from "@material-ui/core";
import { Activity } from "../../src/state/activity.models";

import { Paper } from "@material-ui/core";
import { Table, TableBody, TableRow, TableCell } from "@material-ui/core";

export type ActivityDetailsProps = {
  activity: Activity
}

const useStyles = makeStyles((theme) => ({
  paper: {
    padding: theme.spacing(2),
  },
}))

export default function ActivityDetails({ activity }: ActivityDetailsProps) {
  const classes = useStyles();
  return (
    <Paper className={classes.paper}>
      <Typography variant="overline">
        Activity Information
      </Typography>
      <Table size="small">
        <TableBody>
          <TableRow>
            <TableCell>Free Seats</TableCell>
            <TableCell>{activity.freeSeats}</TableCell>
          </TableRow>
          <TableRow>
            <TableCell>Total Seats</TableCell>
            <TableCell>{activity.totalSeats}</TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </Paper>
  )
}