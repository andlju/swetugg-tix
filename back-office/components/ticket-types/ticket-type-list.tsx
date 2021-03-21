import Link from 'next/link';
import {
  Button,
  TableContainer, Table, TableBody, TableHead, TableRow, TableCell,
  Paper,
  Typography,
  makeStyles
} from "@material-ui/core";
import { TicketType } from "../../store/activities/ticket-type.models";
import { AddTicketType } from "./add-ticket-type";

interface TicketTypeListProps {
  activityId: string,
  ownerId: string,
  loading: boolean,
  ticketTypes: TicketType[],
}

const useStyles = makeStyles((theme) => ({
  paper: {
    padding: theme.spacing(2),
  },
  nameColumnHead: {
    width: "60%"
  },
  reservedColumnHead: {
    width: "10%"
  },
  reservedColumn: {
    textAlign: "right"
  },
  limitColumnHead: {
    width: "10%"
  },
  limitColumn: {
    textAlign: "right"
  },
  actionsColumnHead: {
    width: "20%"
  },
  identifier: {
    color: theme.palette.text.secondary,
    fontVariantCaps: "all-small-caps"
  },
  refreshing : {
    opacity: 0.25
  }
}));

export function TicketTypeList({ activityId, ownerId, loading, ticketTypes }: TicketTypeListProps) {
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
            <TableCell className={classes.reservedColumnHead}>Reserved</TableCell>
            <TableCell className={classes.limitColumnHead}>Limit</TableCell>
            <TableCell className={classes.actionsColumnHead}>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody className={loading ? classes.refreshing : ''}>
          {ticketTypes.map(row => (
            <TableRow key={row.ticketTypeId} hover={true}>
              <TableCell>
                <Typography>{row.name}</Typography>
                <Typography className={classes.identifier}>{row.ticketTypeId}</Typography>
              </TableCell>
              <TableCell className={classes.reservedColumn}>
                <Typography>{row.reserved}</Typography>
              </TableCell>
              <TableCell className={classes.limitColumn}>
                <Typography>{row.limit ?? "-"}</Typography>
              </TableCell>
              <TableCell>
                <Link href={`/activities/${row.activityId}/ticket-types/${row.ticketTypeId}?ownerId=${row.ownerId}`}>
                  <Button variant="outlined">Edit</Button>
                </Link>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>

    <AddTicketType activityId={activityId} ownerId={ownerId} />
  </Paper>);
}

