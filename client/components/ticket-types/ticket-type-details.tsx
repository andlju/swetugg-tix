import { makeStyles, Typography } from "@material-ui/core";
import { ActivityDetailsProps } from "./activity.models";

import { Container } from "@material-ui/core";
import { Table, TableBody, TableRow, TableCell } from "@material-ui/core";

const useStyles = makeStyles((theme) => ({
  root: {
    padding: theme.spacing(0),
  },
}))

interface TicketTypeDetailsProps {
  ticketType: TicketType,

}

export default function TicketTypeDetails({ ticketType }: TicketTypeDetailsProps) {
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
            <TableCell>Limit</TableCell>
            <TableCell>{ticketType.limit ?? "-"}</TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </Container>
  )
}