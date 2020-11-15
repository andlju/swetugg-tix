import { makeStyles, Typography } from "@material-ui/core";

import { Container } from "@material-ui/core";
import { Table, TableBody, TableRow, TableCell } from "@material-ui/core";
import { Activity } from "../activities/activity.models";
import { TicketType } from "./ticket-type.models";

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0),
  },
}))

interface TicketTypeDetailsProps {
  ticketType: TicketType,
  activity: Activity,
}

export function TicketTypeDetails({ ticketType, activity }: TicketTypeDetailsProps) {
  const classes = useStyles();
  return (
    <Container className={classes.root}>
      <Typography variant="overline">
        Ticket Type
      </Typography>
      <Table size="small">
        <TableBody>
          <TableRow>
            <TableCell>Reserved</TableCell>
            <TableCell>{ticketType.reserved}</TableCell>
          </TableRow>
          <TableRow>
            <TableCell>Limit / Total seats</TableCell>
            <TableCell>{ticketType.limit ?? "-"} / {activity.totalSeats}</TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </Container>
  )
}