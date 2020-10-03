import Link from 'next/link';
import {
  Button,
  TableContainer, Table, TableBody, TableHead, TableRow, TableCell,
  Paper,
  Typography,
  makeStyles
} from "@material-ui/core";
import { TicketType } from "./ticket-type.models";
import AddTicketType from "./add-ticket-type";

interface TicketTypeListProps {
  activityId: string,
  ticketTypes: TicketType[]
}

const useStyles = makeStyles((theme) => ({
  paper: {
    padding: theme.spacing(2),
  },
  nameColumnHead: {
    width: "70%"
  },
  actionsColumnHead: {
    width: "30%"
  },
  identifier: {
    color: theme.palette.text.secondary,
    fontVariantCaps: "all-small-caps"
  }
}));

export default function TicketTypeList({ ticketTypes, activityId }: TicketTypeListProps) {
  const classes = useStyles();
  return (<Paper className={classes.paper}>
    <Typography variant="overline">
      Ticket Types
    </Typography>
    <TableContainer>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell className={classes.nameColumnHead}>Name</TableCell>
            <TableCell className={classes.actionsColumnHead}>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {ticketTypes.map(row => (
            <TableRow key={row.ticketTypeId} hover={true}>
              <TableCell>
                <Typography>{row.name}</Typography>
                <Typography className={classes.identifier}>{row.ticketTypeId}</Typography>
              </TableCell>
              <TableCell>
                <Link href={`/activities/${row.activityId}/ticket-types/${row.ticketTypeId}`}>
                  <Button variant="outlined">Edit</Button>
                </Link>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
    <AddTicketType activityId={activityId} />
  </Paper>);
}

