import { makeStyles, Typography } from "@material-ui/core";
import { ActivityDetailsProps } from "./activity.models";

import { Container } from "@material-ui/core";
import { Table, TableBody, TableRow, TableCell } from "@material-ui/core";

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0),
  },
}))

export default function ActivityDetails({ activity }: ActivityDetailsProps) {
  const classes = useStyles();
  return (
    <Container className={classes.root}>
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
    </Container>
  )
}